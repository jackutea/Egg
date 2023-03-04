using System;
using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using System.Collections.Generic;

namespace TiedanSouls.World.Domain {

    public class WorldFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldDomain worldDomain;

        public WorldFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void EnterLobby() {
            // Config
            var gameConfigTM = infraContext.TemplateCore.GameConfigTM;

            // Spawn Field
            var firstFieldTypeID = gameConfigTM.lobbyFieldTypeID;
            var fieldDomain = worldDomain.FieldDomain;
            if (!fieldDomain.TryGetOrSpawnField(firstFieldTypeID, out var field)) {
                TDLog.Error($"进入大厅失败! 无法生成大厅场景! TypeID {firstFieldTypeID}");
                return;
            }

            // Check
            var fieldType = field.FieldType;
            if (fieldType != FieldType.Lobby) {
                TDLog.Error($"进入大厅失败! 场景类型不是大厅! TypeID {firstFieldTypeID} / FieldType {fieldType}");
                return;
            }

            // World FSM
            var stateEntity = worldContext.StateEntity;
            stateEntity.EnterState_Lobby(field.TypeID);

            // Lobby Character
            TDLog.Log($"大厅人物生成开始 -------------------------------------------- ");
            var spawnModelArray = field.SpawnModelArray;
            var spawnCount = spawnModelArray?.Length;
            var roleDomain = worldDomain.RoleDomain;
            for (int i = 0; i < spawnCount; i++) {
                var spawnModel = spawnModelArray[i];
                var entityType = spawnModel.entityType;
                var typeID = spawnModel.typeID;
                var roleControlType = spawnModel.controlType;
                var allyType = spawnModel.allyType;
                var ownerRoleSpawnPos = spawnModel.pos;
                if (entityType == EntityType.Role) {
                    var role = roleDomain.SpawnRole(roleControlType, typeID, allyType, ownerRoleSpawnPos);
                    TDLog.Log($"人物: AllyType {allyType} / ControlType {role.ControlType} / TypeID {typeID} / RoleName {role.RoleName}");
                } else {
                    TDLog.Error("Not Handle Yet!");
                }
            }
            TDLog.Log($"大厅人物生成结束 -------------------------------------------- ");

            TDLog.Log($"大厅物件生成开始 -------------------------------------------- ");
            var lobbyItemTypeIDs = gameConfigTM.lobbyItemTypeIDs;
            var itemCount = lobbyItemTypeIDs?.Length;
            var itemSpawnPosArray = field.ItemSpawnPosArray;
            for (int i = 0; i < itemCount; i++) {
                if (i >= itemSpawnPosArray.Length) {
                    TDLog.Error("物件生成位置不足!");
                    break;
                }

                var typeID = lobbyItemTypeIDs[i];
                var itemSpawnPos = itemSpawnPosArray[i];
                var itemEntity = worldContext.WorldFactory.SpawnItemEntity(typeID, itemSpawnPos, firstFieldTypeID);
                TDLog.Log($"物件: EntityID: {itemEntity.EntityD} / TypeID {itemEntity.TypeID} / ItemType {itemEntity.ItemType} / TypeIDForPickUp {itemEntity.TypeIDForPickUp}");
            }
            TDLog.Log($"大厅物件生成结束 -------------------------------------------- ");

            // Spawn TieDan 
            var tieDanRoleTypeID = gameConfigTM.tiedanRoleTypeID;
            var owner = roleDomain.SpawnRole(ControlType.Player, tieDanRoleTypeID, AllyType.Player, new Vector2(5, 5));
            owner.WeaponSlotCom.SetWeaponActive(false);

            // Set Camera 
            var cameraSetter = infraContext.CameraCore.SetterAPI;
            cameraSetter.Follow_Current(owner.transform, new Vector3(0, 0, -10), EasingType.Immediate, 1f, EasingType.Linear, 1f);
            cameraSetter.Confiner_Set_Current(true, field.transform.position, (Vector2)field.transform.position + field.ConfinerSize);

        }

        public void ApplyWorldState(float dt) {
            var stateEntity = worldContext.StateEntity;
            var worldStatus = stateEntity.Status;
            if (worldStatus == WorldFSMState.Lobby) {
                ApplyWorldState_Lobby(dt);
            } else if (worldStatus == WorldFSMState.Battle) {
                ApplyWorldState_Battle(dt);
            } else if (worldStatus == WorldFSMState.Store) {
                ApplyWorldState_Store(dt);
            } else if (worldStatus == WorldFSMState.Loading) {
                ApplyWorldState_Loading(dt);
            } else {
                TDLog.Error($"未知的World状态! Status: {worldStatus}");
            }

            // Clear Input
            var roleRepo = worldContext.RoleRepo;
            roleRepo.ForeachAll((role) => {
                role.InputCom.Reset();
            });
        }

        public void ApplyWorldState_Lobby(float dt) {
            DoBasicLogic(dt);

            var stateEntity = worldContext.StateEntity;
            var lobbyStateModel = stateEntity.LobbyStateModel;
            if (lobbyStateModel.IsEntering) {
                lobbyStateModel.SetIsEntering(false);
            }

            // 检查玩家是否满足离开条件: 消灭所有敌人、拾取奖励、走到出口并按下离开键
            if (!IsTieDanWantToLeave(out var door)) {
                return;
            }

            var playerRole = worldContext.RoleRepo.PlayerRole;
            if (!playerRole.WeaponSlotCom.HasWeapon()) {
                TDLog.Warning("没有武器你就是个'卤蛋'!!!");
                return;
            }

            var nextFieldTypeID = door.fieldTypeID;
            var fieldDomain = worldDomain.FieldDomain;
            if (!fieldDomain.TryGetOrSpawnField(nextFieldTypeID, out var nextField)) {
                TDLog.Error($"请检查配置! 下一场景不存在! FieldTypeID: {nextFieldTypeID}");
                return;
            }

            if (nextField.FieldType == FieldType.BattleField) {
                var doorIndex = door.doorIndex;
                var curFieldTypeID = stateEntity.CurFieldTypeID;
                stateEntity.EnterState_Loading(curFieldTypeID, nextFieldTypeID, doorIndex);
                return;
            }
        }

        public void ApplyWorldState_Battle(float dt) {
            DoBasicLogic(dt);

            var playerRole = worldContext.RoleRepo.PlayerRole;
            var stateEntity = worldContext.StateEntity;
            var battleFieldStateModel = stateEntity.BattleStateModel;
            if (battleFieldStateModel.IsEntering) {
                battleFieldStateModel.SetIsEntering(false);
                playerRole.WeaponSlotCom.SetWeaponActive(true);
            }

            // FieldFSM
            var curFieldTypeID = stateEntity.CurFieldTypeID;
            var fieldDomain = worldDomain.FieldDomain;
            fieldDomain.TickFSM(curFieldTypeID, dt);

            // 检查玩家是否满足离开条件: 消灭所有敌人、拾取奖励、走到出口并按下离开键
            if (!IsTieDanWantToLeave(out var door)) {
                return;
            }

            // TODO: 检查是否有奖励未拾取
            // TODO: 检查是否有敌人未消灭

            var nextFieldTypeID = door.fieldTypeID;
            if (!fieldDomain.TryGetOrSpawnField(nextFieldTypeID, out var nextField)) {
                TDLog.Error($"请检查配置! 下一场景不存在! FieldTypeID: {nextFieldTypeID}");
                return;
            }

            var doorIndex = door.doorIndex;
            stateEntity.EnterState_Loading(curFieldTypeID, nextFieldTypeID, doorIndex);

        }

        void ApplyWorldState_Store(float dt) {
            DoBasicLogic(dt);
        }

        void DoBasicLogic(float dt) {
            var roleDomain = worldDomain.RoleDomain;
            var roleFSMDomain = worldDomain.RoleFSMDomain;
            var stateEntity = worldContext.StateEntity;
            var roleRepo = worldContext.RoleRepo;
            roleRepo.ForeachAll((role) => {
                // AI
                if (role.ControlType == ControlType.AI) {
                    role.AIStrategy.Tick(dt);
                }

                // Role FSM
                roleFSMDomain.Tick(role, dt);
            });

            // Physics
            var phxDomain = worldDomain.WorldPhysicsDomain;
            phxDomain.Tick(dt);
        }

        void ApplyWorldState_Loading(float dt) {
            var stateEntity = worldContext.StateEntity;
            var loadingStateModel = stateEntity.LoadingStateModel;
            var loadingFieldTypeID = loadingStateModel.NextFieldTypeID;
            var fieldDomain = worldDomain.FieldDomain;

            if (loadingStateModel.IsEntering) {
                loadingStateModel.SetIsEntering(false);

                // 隐藏当前场景
                var curFieldTypeID = stateEntity.CurFieldTypeID;
                fieldDomain.HideField(curFieldTypeID);

                // 隐藏当前场景内物件
                var itemRepo = worldContext.ItemRepo;
                itemRepo.HideAllItemsInField(curFieldTypeID);

                // 隐藏当前场景内AI角色
                var roleRepo = worldContext.RoleRepo;
                roleRepo.HideAllAIRolesInField(curFieldTypeID);

                // 加载下一个场景
                if (!fieldDomain.TryGetOrSpawnField(loadingFieldTypeID, out var field)) {
                    TDLog.Error($"场景不存在! FieldTypeID: {loadingFieldTypeID}");
                    return;
                }

                loadingStateModel.SetIsLoadingComplete(true);

            }

            if (loadingStateModel.IsLoadingComplete) {
                _ = worldContext.FieldRepo.TryGet(loadingFieldTypeID, out var field);

                // 根据场景类型进入不同的世界状态
                if (field.FieldType == FieldType.BattleField) {
                    stateEntity.EnterState_Battle(loadingFieldTypeID);
                } else if (field.FieldType == FieldType.Store) {
                    stateEntity.EnterState_Store(loadingFieldTypeID);
                } else if (field.FieldType == FieldType.Lobby) {
                    stateEntity.EnterState_Lobby(loadingFieldTypeID);
                } else {
                    TDLog.Warning($"未处理的场景类型: {field.FieldType}");
                }

                // 显示场景内所有实体
                var spawnModelArray = field.SpawnModelArray;
                var len = spawnModelArray.Length;
                for (int i = 0; i < len; i++) {
                    var spawnModel = spawnModelArray[i];
                    var entityType = spawnModel.entityType;
                    var typeID = spawnModel.typeID;
                    var controlType = spawnModel.controlType;
                    var allyType = spawnModel.allyType;
                    var spawnPos = spawnModel.pos;

                    if (entityType == EntityType.Role) {
                        var roleDomain = worldDomain.RoleDomain;
                        var role = roleDomain.SpawnRole(controlType, typeID, allyType, spawnPos);
                    } else {
                        TDLog.Error($"未处理 EntityType: {entityType}");
                    }
                }

                // 设置铁蛋初始场景位置
                var playerRole = worldContext.RoleRepo.PlayerRole;
                var doorIndex = loadingStateModel.DoorIndex;
                _ = field.TryFindDoorByIndex(doorIndex, out var door);
                var doorPos = door.pos;
                doorPos.y = playerRole.GetPos_Logic().y;
                playerRole.SetPos_Logic(doorPos);
                playerRole.SyncRenderer();

                field.FSMComponent.Enter_Ready();
            }
        }

        bool IsTieDanWantToLeave(out FieldDoorModel door) {
            door = default;
            var stateEntity = worldContext.StateEntity;
            // 检查玩家InputComponent是否输入了进入战场的指令
            var fieldRepo = worldContext.FieldRepo;
            var playerRole = worldContext.RoleRepo.PlayerRole;
            var inputCom = playerRole.InputCom;
            if (inputCom.HasInput_Basic_Pick) {
                var curFieldTypeID = stateEntity.CurFieldTypeID;
                if (!fieldRepo.TryGet(curFieldTypeID, out var curField)) {
                    TDLog.Error($"请检查配置! 当前场景不存在! FieldTypeID: {stateEntity.CurFieldTypeID}");
                    return false;
                }

                // 检查玩家是否在场景的门口
                var allDoors = curField.FieldDoorArray;
                var count = allDoors?.Length;
                for (int i = 0; i < count; i++) {
                    var d = allDoors[i];
                    var pos = d.pos;
                    var rolePos = playerRole.GetPos_LogicRoot();
                    if (Vector2.SqrMagnitude(pos - rolePos) > 1f) {
                        continue;
                    }

                    door = d;
                    return true;
                }
            }

            return false;
        }

    }
}
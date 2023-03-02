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

            // Lobby Character
            TDLog.Log($"大厅人物生成开始 -------------------------------------------- ");
            var spawnModelArray = field.SpawnModelArray;
            var spawnCount = spawnModelArray?.Length;
            var roleDomain = worldDomain.RoleDomain;
            for (int i = 0; i < spawnCount; i++) {
                var spawnModel = spawnModelArray[i];
                var entityType = spawnModel.entityType;
                var typeID = spawnModel.typeID;
                var roleControlType = spawnModel.roleControlType;
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
                var itemEntity = worldContext.WorldFactory.SpawnItemEntity(typeID, itemSpawnPos);
                TDLog.Log($"物件: EntityID: {itemEntity.ID} / TypeID {itemEntity.TypeID} / ItemType {itemEntity.ItemType} / TypeIDForPickUp {itemEntity.TypeIDForPickUp}");
            }
            TDLog.Log($"大厅物件生成结束 -------------------------------------------- ");

            // Spawn TieDan 
            var tieDanRoleTypeID = gameConfigTM.tiedanRoleTypeID;
            var owner = roleDomain.SpawnRole(RoleControlType.Player, tieDanRoleTypeID, AllyType.Player, new Vector2(5, 5));

            // Set Camera 
            var cameraSetter = infraContext.CameraCore.SetterAPI;
            cameraSetter.Follow_Current(owner.transform, new Vector3(0, 0, -10), EasingType.Immediate, 1f, EasingType.Linear, 1f);
            cameraSetter.Confiner_Set_Current(true, field.transform.position, (Vector2)field.transform.position + field.ConfinerSize);

            // World State
            var stateEntity = worldContext.StateEntity;
            stateEntity.EnterState_Lobby(owner.ID, field.TypeID);
        }

        public void ApplyWorldState(float dt) {
            var stateEntity = worldContext.StateEntity;
            var worldStatus = stateEntity.Status;
            if (worldStatus == WorldFSMState.Lobby) {
                ApplyWorldState_Lobby(dt);
            } else if (worldStatus == WorldFSMState.BattleField) {
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
            ApplyBasicLogic(dt);

            var stateEntity = worldContext.StateEntity;
            var lobbyStateModel = stateEntity.LobbyStateModel;
            if (lobbyStateModel.IsEntering) {
                lobbyStateModel.SetIsEntering(false);
            }

            // 检查玩家InputComponent是否输入了进入战场的指令
            var fieldRepo = worldContext.FieldRepo;
            var playerRole = worldContext.RoleRepo.PlayerRole;
            var inputCom = playerRole.InputCom;
            if (inputCom.HasInput_Basic_Pick) {
                var curFieldTypeID = stateEntity.CurFieldTypeID;
                if (!fieldRepo.TryGet(curFieldTypeID, out var curField)) {
                    TDLog.Error($"当前场景不存在! FieldTypeID: {stateEntity.CurFieldTypeID}");
                    return;
                }

                var doors = curField.FieldDoorArray;
                var doorCount = doors?.Length;
                for (int i = 0; i < doorCount; i++) {
                    var door = doors[i];
                    var pos = door.pos;
                    var rolePos = playerRole.GetPos_LogicRoot();
                    if (Vector2.SqrMagnitude(pos - rolePos) > 1f) {
                        continue;
                    }

                    var nextFieldTypeID = door.fieldTypeID;
                    var fieldDomain = worldDomain.FieldDomain;
                    if (!fieldDomain.TryGetOrSpawnField(nextFieldTypeID, out var nextField)) {
                        TDLog.Error($"场景不存在! FieldTypeID: {nextFieldTypeID}");
                        return;
                    }

                    if (nextField.FieldType == FieldType.BattleField) {
                        var doorIndex = door.doorIndex;
                        stateEntity.EnterState_Loading(curFieldTypeID, nextFieldTypeID, doorIndex);
                        return;
                    }
                }

                TDLog.Warning("没有找到合适的门离开!");
            }

        }

        public void ApplyWorldState_Battle(float dt) {
            ApplyBasicLogic(dt);

            var stateEntity = worldContext.StateEntity;
            var battleFieldStateModel = stateEntity.BattleStateModel;
            if (battleFieldStateModel.IsEntering) {
                battleFieldStateModel.SetIsEntering(false);
            }
        }

        void ApplyWorldState_Store(float dt) {
            ApplyBasicLogic(dt);
        }

        void ApplyBasicLogic(float dt) {
            var roleDomain = worldDomain.RoleDomain;
            var roleFSMDomain = worldDomain.RoleFSMDomain;
            var stateEntity = worldContext.StateEntity;
            var roleRepo = worldContext.RoleRepo;
            roleRepo.ForeachAll((role) => {
                // AI
                if (role.ControlType == RoleControlType.AI) {
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

            if (loadingStateModel.IsEntering) {
                loadingStateModel.SetIsEntering(false);

                var fieldDomain = worldDomain.FieldDomain;

                // 隐藏当前场景
                var curFieldTypeID = stateEntity.CurFieldTypeID;
                fieldDomain.HideField(curFieldTypeID);

                // 显示下一个场景
                if (!fieldDomain.TryGetOrSpawnField(loadingFieldTypeID, out var field)) {
                    TDLog.Error($"场景不存在! FieldTypeID: {loadingFieldTypeID}");
                    return;
                }

                loadingStateModel.SetIsLoadingComplete(true);
            }

            if (loadingStateModel.IsLoadingComplete) {
                stateEntity.EnterState_Battle(loadingFieldTypeID);
            }

        }


    }
}
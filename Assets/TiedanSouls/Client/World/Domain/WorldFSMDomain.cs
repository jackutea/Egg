using System;
using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.Generic;

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

        public void StartGame() {
            var gameConfigTM = infraContext.TemplateCore.GameConfigTM;
            var firstFieldTypeID = gameConfigTM.lobbyFieldTypeID;

            var stateEntity = worldContext.StateEntity;
            stateEntity.EnterState_Loading(-1, firstFieldTypeID, 0);
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

                // 生成铁蛋 
                var gameConfigTM = infraContext.TemplateCore.GameConfigTM;
                var tieDanRoleTypeID = gameConfigTM.tiedanRoleTypeID;

                var roleDomain = worldDomain.RoleDomain;
                var owner = roleDomain.SpawnRole(ControlType.Player, tieDanRoleTypeID, AllyType.Player, new Vector2(5, 5));

                // 大厅禁用武器
                owner.WeaponSlotCom.SetWeaponActive(false);

                // 设置相机 
                _ = worldContext.FieldRepo.TryGet(stateEntity.CurFieldTypeID, out var field);
                var cameraSetter = infraContext.CameraCore.SetterAPI;
                cameraSetter.Follow_Current(owner.transform, new Vector3(0, 0, -10), EasingType.Immediate, 1f, EasingType.Linear, 1f);
                cameraSetter.Confiner_Set_Current(true, field.transform.position, (Vector2)field.transform.position + field.ConfinerSize);
            }

            // 刷新关卡状态机
            var fieldFSMDomain = worldDomain.FieldFSMDomain;
            fieldFSMDomain.TickFSM_CurrentField(dt);

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

            // 刷新关卡状态机
            var fieldFSMDomain = worldDomain.FieldFSMDomain;
            fieldFSMDomain.TickFSM_CurrentField(dt);

            // 检查玩家是否满足离开条件: 消灭所有敌人、拾取奖励、走到出口并按下离开键
            if (!IsTieDanWantToLeave(out var door)) {
                return;
            }

            // TODO: 检查是否有奖励未拾取
            // TODO: 检查是否有敌人未消灭

            var nextFieldTypeID = door.fieldTypeID;
            var fieldDomain = worldDomain.FieldDomain;
            if (!fieldDomain.TryGetOrSpawnField(nextFieldTypeID, out var nextField)) {
                TDLog.Error($"请检查配置! 下一场景不存在! FieldTypeID: {nextFieldTypeID}");
                return;
            }

            var doorIndex = door.doorIndex;
            var curFieldTypeID = stateEntity.CurFieldTypeID;
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

                // 判断是否场景是否已经生成过
                var fieldRepo = worldContext.FieldRepo;
                if (!fieldRepo.TryGet(loadingFieldTypeID, out var field)) {
                    fieldDomain.TryGetOrSpawnField(loadingFieldTypeID, out field);
                }

                if (field == null) {
                    TDLog.Error($"场景不存在! FieldTypeID: {loadingFieldTypeID}");
                    return;
                }

                loadingStateModel.SetIsLoadingComplete(true);
            }

            if (loadingStateModel.IsLoadingComplete) {
                _ = worldContext.FieldRepo.TryGet(loadingFieldTypeID, out var field);

                // 世界状态切换
                if (field.FieldType == FieldType.BattleField) {
                    stateEntity.EnterState_Battle(loadingFieldTypeID);
                } else if (field.FieldType == FieldType.Store) {
                    stateEntity.EnterState_Store(loadingFieldTypeID);
                } else if (field.FieldType == FieldType.Lobby) {
                    stateEntity.EnterState_Lobby(loadingFieldTypeID);
                } else {
                    TDLog.Warning($"未处理的场景类型: {field.FieldType}");
                }

                // 关卡状态切换
                var doorIndex = loadingStateModel.DoorIndex;
                _ = field.TryFindDoorByIndex(doorIndex, out var door);
                field.FSMComponent.Enter_Ready(door);
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
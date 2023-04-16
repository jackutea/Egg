using System;
using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Generic;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client.Domain {

    public class WorldFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldDomain;

        public WorldFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void StartGame() {
            var gameConfigTM = infraContext.TemplateCore.GameConfigTM;
            var lobbyFieldTypeID = gameConfigTM.lobbyFieldTypeID;

            var stateEntity = worldContext.StateEntity;
            stateEntity.EnterState_Loading(-1, lobbyFieldTypeID, 0);
        }

        public void ApplyWorldState(float dt) {
            var stateEntity = worldContext.StateEntity;
            var worldState = stateEntity.State;
            if (worldState == WorldFSMState.Exit) {
                return;
            }

            if (worldState == WorldFSMState.Lobby) {
                Apply_Lobby(dt);
            } else if (worldState == WorldFSMState.Battle) {
                Apply_Battle(dt);
            } else if (worldState == WorldFSMState.Store) {
                Apply_Store(dt);
            } else if (worldState == WorldFSMState.Loading) {
                Apply_Loading(dt);
            }

            // Clear Input
            var roleRepo = worldContext.RoleRepo;
            roleRepo.Foreach_All((role) => {
                role.InputCom.Reset();
            });
        }

        public void Apply_Lobby(float dt) {
            var roleRepo = worldContext.RoleRepo;
            RoleEntity playerRole = roleRepo.PlayerRole;

            var stateEntity = worldContext.StateEntity;
            var curFieldTypeID = stateEntity.CurFieldTypeID;

            var lobbyStateModel = stateEntity.LobbyStateModel;
            if (lobbyStateModel.IsEntering) {
                lobbyStateModel.SetIsEntering(false);
                // 生成铁蛋 
                var gameConfigTM = infraContext.TemplateCore.GameConfigTM;
                var tieDanRoleTypeID = gameConfigTM.tiedanRoleTypeID;

                var roleDomain = worldDomain.RoleDomain;
                if (playerRole == null) {
                    roleDomain.TrySpawnRole(curFieldTypeID,
                                            new EntitySpawnModel { allyType = AllyType.One, controlType = ControlType.Player, typeID = tieDanRoleTypeID },
                                            out playerRole);
                }
                playerRole.Reset();
                playerRole.FSMCom.Enter_Idle();

                roleDomain.Show(playerRole);
                playerRole.HudSlotCom.ShowHUD();

                // 设置相机 
                _ = worldContext.FieldRepo.TryGet(curFieldTypeID, out var field);
                var cameraSetter = infraContext.CameraCore.SetterAPI;
                cameraSetter.Follow_Current(playerRole.transform, new Vector3(0, 0, -10), EasingType.Immediate, 1f, EasingType.Linear, 1f);
                cameraSetter.Confiner_Set_Current(true, field.ModGO.transform.position, (Vector2)field.ModGO.transform.position + field.ConfinerSize);
            }

            var phxDomain = worldDomain.PhysicalDomain;
            phxDomain.Tick(dt);

            TickAllFSM(dt);

            // 检查玩家是否满足离开条件: 拾取了武器
            if (!IsTieDanWantToLeave(out var door)) return;

            if (!playerRole.WeaponSlotCom.HasWeapon()) {
                TDLog.Warning("请选择你使用的武器!");
                return;
            }

            var nextFieldTypeID = door.fieldTypeID;
            var fieldDomain = worldDomain.FieldDomain;
            if (!fieldDomain.TryGetOrSpawnField(nextFieldTypeID, out var nextField)) {
                TDLog.Error($"请检查配置! 下一关卡不存在! FieldTypeID: {nextFieldTypeID}");
                return;
            }

            var doorIndex = door.doorIndex;
            stateEntity.EnterState_Loading(stateEntity.CurFieldTypeID, nextFieldTypeID, doorIndex);
        }

        public void Apply_Battle(float dt) {
            var roleRepo = worldContext.RoleRepo;
            var playerRole = roleRepo.PlayerRole;
            var stateEntity = worldContext.StateEntity;
            var battleFieldStateModel = stateEntity.BattleStateModel;
            if (battleFieldStateModel.IsEntering) {
                battleFieldStateModel.SetIsEntering(false);
                playerRole.WeaponSlotCom.SetWeaponActive(true);
            }

            var phxDomain = worldDomain.PhysicalDomain;
            phxDomain.Tick(dt);

            TickAllFSM(dt);

            // 检查玩家是否满足离开条件: 消灭所有敌人、拾取奖励、走到出口并按下离开键
            if (!IsTieDanWantToLeave(out var door)) return;

            // 检查是否有敌人未消灭
            var fieldDomain = worldDomain.FieldDomain;
            var curField = fieldDomain.GetCurField();
            var fieldFSM = curField.FSMComponent;
            if (fieldFSM.State != FieldFSMState.Finished) {
                TDLog.Warning("还有敌人未消灭-------------------");
                return;
            }

            // TODO: 检查是否有奖励未拾取
            var nextFieldTypeID = door.fieldTypeID;
            var doorIndex = door.doorIndex;
            stateEntity.EnterState_Loading(worldContext.StateEntity.CurFieldTypeID, nextFieldTypeID, doorIndex);
        }

        void Apply_Store(float dt) {
        }

        void Apply_Loading(float dt) {
            var stateEntity = worldContext.StateEntity;
            var loadingStateModel = stateEntity.LoadingStateModel;
            var loadingFieldTypeID = loadingStateModel.NextFieldTypeID;
            var fieldDomain = worldDomain.FieldDomain;

            if (loadingStateModel.IsEntering) {
                loadingStateModel.SetIsEntering(false);

                // 判断是否关卡是否已经生成过
                var fieldRepo = worldContext.FieldRepo;
                if (!fieldRepo.TryGet(loadingFieldTypeID, out var field)) {
                    fieldDomain.TryGetOrSpawnField(loadingFieldTypeID, out field);
                }

                if (field == null) {
                    TDLog.Error($"关卡不存在! FieldTypeID: {loadingFieldTypeID}");
                    return;
                }

                loadingStateModel.SetIsLoadingCompleted(true);
                field.Hide();
            }

            if (loadingStateModel.IsLoadingCompleted) {
                if (loadingStateModel.completeLoadingDelayFrame > 0) {
                    loadingStateModel.completeLoadingDelayFrame--;
                    return;
                }

                // Recycle 当前关卡 
                var curFieldTypeID = stateEntity.CurFieldTypeID;
                fieldDomain.RecycleField(curFieldTypeID);

                // Recycle 当前关卡 物件
                var itemRepo = worldContext.ItemRepo;
                itemRepo.RecycleFieldItems(curFieldTypeID);

                // Recycle 当前关卡 AI角色
                var roleDomain = worldDomain.RoleDomain;
                roleDomain.RecycleFieldRoles(curFieldTypeID);

                // Recycle 当前关卡 子弹
                var bulletDomain = worldContext.RootDomain.BulletDomain;
                bulletDomain.RecycleFieldBullets(curFieldTypeID);

                // Recycle 当前关卡 弹幕
                var projectileDomain = worldContext.RootDomain.ProjectileDomain;
                projectileDomain.RecycleProjectiles(curFieldTypeID);

                // 显示下一关卡
                _ = worldContext.FieldRepo.TryGet(loadingFieldTypeID, out var field);
                field.Show();

                // 世界状态切换
                if (field.FieldType == FieldType.BattleField) {
                    stateEntity.EnterState_Battle(loadingFieldTypeID);
                } else if (field.FieldType == FieldType.Store) {
                    stateEntity.EnterState_Store(loadingFieldTypeID);
                } else if (field.FieldType == FieldType.Lobby) {
                    stateEntity.EnterState_Lobby(loadingFieldTypeID);
                } else {
                    TDLog.Warning($"未处理的关卡类型: {field.FieldType}");
                }

                // 关卡状态切换
                var doorIndex = loadingStateModel.DoorIndex;
                _ = field.TryFindDoorByIndex(doorIndex, out var door);
                field.FSMComponent.Enter_Ready(door);
            }
        }

        void TickAllFSM(float dt) {
            var stateEntity = worldContext.StateEntity;
            var curFieldTypeID = stateEntity.CurFieldTypeID;

            // 刷新 关卡 状态机
            var fieldFSMDomain = worldDomain.FieldFSMDomain;
            fieldFSMDomain.TickFSM(dt);

            // 刷新 角色 状态机(当前关卡内)
            var roleFSMDomain = worldDomain.RoleFSMDomain;
            roleFSMDomain.TickFSM(curFieldTypeID, dt);

            // 刷新 弹幕 状态机(当前关卡内)
            var projectileFSMDomain = worldDomain.ProjectileFSMDomain;
            projectileFSMDomain.TickFSM(curFieldTypeID, dt);

            // 刷新 子弹 状态机(当前关卡内)
            var bulletFSMDomain = worldDomain.BulletFSMDomain;
            bulletFSMDomain.TickFSM(curFieldTypeID, dt);
        }

        bool IsTieDanWantToLeave(out FieldDoorModel door) {
            door = default;
            var stateEntity = worldContext.StateEntity;
            // 检查玩家InputComponent是否输入了进入战场的指令
            var fieldRepo = worldContext.FieldRepo;
            var playerRole = worldContext.RoleRepo.PlayerRole;
            var inputCom = playerRole.InputCom;
            if (inputCom.InputPick) {
                var curFieldTypeID = stateEntity.CurFieldTypeID;
                if (!fieldRepo.TryGet(curFieldTypeID, out var curField)) {
                    TDLog.Error($"请检查配置! 当前关卡不存在! FieldTypeID: {stateEntity.CurFieldTypeID}");
                    return false;
                }

                // 检查玩家是否在关卡的门口
                var allDoors = curField.FieldDoorArray;
                var count = allDoors?.Length;
                for (int i = 0; i < count; i++) {
                    var d = allDoors[i];
                    var pos = d.pos;
                    var rolePos = playerRole.LogicRootPos;
                    if (Vector3.SqrMagnitude(pos - rolePos) > 1f) {
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
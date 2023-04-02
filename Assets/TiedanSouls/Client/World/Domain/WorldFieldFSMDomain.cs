using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldFieldFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldFieldFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public void TickFSM(float dt) {
            var fieldRepo = worldContext.FieldRepo;
            fieldRepo.Foreach((field) => {
                TickFSM_CurrentField(field, dt);
            });
        }

        void TickFSM_CurrentField(FieldEntity field, float dt) {
            var fsm = field.FSMComponent;
            if (fsm.IsExiting) return;

            var state = fsm.State;
            if (state == FieldFSMState.Ready) {
                ApplyFSMState_Ready(field, fsm, dt);
            } else if (state == FieldFSMState.Spawning) {
                ApplyFSMState_Spawning(field, dt);
            } else if (state == FieldFSMState.Finished) {
                ApplyFSMState_Finished(fsm, dt);
            }
        }

        void ApplyFSMState_Ready(FieldEntity field, FieldFSMComponent fsm, float dt) {
            var readyModel = fsm.ReadyModel;
            if (readyModel.IsEntering) {
                readyModel.SetIsEntering(false);

                // 关卡切换时 角色相关刷新
                var curFieldTypeID = field.IDCom.TypeID;
                var playerRole = worldContext.RoleRepo.PlayerRole;

                // - FromFieldTypeID
                playerRole.SetFromFieldTypeID(curFieldTypeID);

                // - 位置
                var door = readyModel.EnterDoorModel;
                var doorPos = door.pos;
                doorPos.y = playerRole.LogicPos.y;
                playerRole.SetLogicPos(doorPos);

                // - GO Name
                playerRole.name = $"主角_{playerRole.IDCom}";

                // - 表现层同步当前位置
                var roleDomain = worldContext.RootDomain.RoleDomain;
                roleDomain.Renderer_Sync(playerRole);

                // - 测试buff
                var buffSlotCom = playerRole.BuffSlotCom;
                var buffDomain = worldContext.RootDomain.BuffDomain;
                if (!buffSlotCom.HasSameTypeBuff(9999)) {
                    if (buffDomain.TrySpawn(9999, playerRole.IDCom.ToArgs(), out var buff)) {
                        buffSlotCom.Add(buff);
                    }
                }
                if (!buffSlotCom.HasSameTypeBuff(99991)) {
                    if (buffDomain.TrySpawn(99991, playerRole.IDCom.ToArgs(), out var buff)) {
                        buffSlotCom.Add(buff);
                        roleDomain.ReduceHP_Percentage(playerRole, 0.9f);
                    }
                }
            }

            // TODO: 触发生成敌人的前置条件 如 玩家进入某个区域 或者 玩家点击某个按钮 或者 玩家等待一段时间 或者 对话结束......
            var totalSpawnCount = field.EntitySpawnCtrlModelArray?.Length ?? 0;
            fsm.Enter_Spawning(totalSpawnCount);
        }

        void ApplyFSMState_Spawning(FieldEntity field, float dt) {
            var idCom = field.IDCom;
            var fieldTypeID = idCom.TypeID;
            var fsm = field.FSMComponent;
            var stateModel = fsm.SpawningModel;
            var roleRepo = worldContext.RoleRepo;
            var stateEntity = worldContext.StateEntity;
            var curFieldTypeID = stateEntity.CurFieldTypeID;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);

                // 判断前置条件: 是否是重复加载关卡

                if (roleRepo.HasAI(fieldTypeID)) {
                    var roleDomain = worldContext.RootDomain.RoleDomain;
                    roleDomain.ResetAllAIs(fieldTypeID, false);
                    stateModel.SetIsRespawning(true);
                }

                if (!stateModel.IsRespawning) {
                    // TODO: 大厅物件生成逻辑也统一到SpawnModel里面去(这样就不需要特殊处理了，物件并非只有大厅有)
                    if (field.FieldType == FieldType.Lobby) {
                        TDLog.Log($"大厅物件生成开始 -------------------------------------------- ");
                        var gameConfigTM = infraContext.TemplateCore.GameConfigTM;
                        var lobbyItemTypeIDArray = gameConfigTM.lobbyItemTypeIDs;
                        var itemCount = lobbyItemTypeIDArray?.Length;
                        var itemSpawnPosArray = field.ItemSpawnPosArray;
                        for (int i = 0; i < itemCount; i++) {
                            if (i >= itemSpawnPosArray.Length) {
                                TDLog.Error("物件生成位置不足!");
                                break;
                            }

                            var typeID = lobbyItemTypeIDArray[i];
                            var itemSpawnPos = itemSpawnPosArray[i];
                            worldContext.RootDomain.ItemDomain.SpawnItemEntity(typeID, itemSpawnPos, field.IDCom.ToArgs());
                        }
                        TDLog.Log($"大厅物件生成结束 -------------------------------------------- ");
                    }
                }

            }

            // 刷新当前关卡存活敌人数量
            int aliveEnemyCount = 0;
            int aliveBossCount = 0;
            roleRepo.Foreach_EnemyOfPlayer(curFieldTypeID,
            (enemy) => {
                if (!enemy.RoleAttributeCom.IsDead()) {
                    aliveEnemyCount++;
                    if (enemy.IsBoss) aliveBossCount++;
                }
            });
            stateModel.aliveEnemyCount = aliveEnemyCount;

            // 杀死当前所有敌人(不包括Boss)才能推进关卡继续生成敌人
            var aliveEnemyCount_excludeBoss = aliveEnemyCount - aliveBossCount;
            if (stateModel.IsSpawningPaused) {
                if (aliveEnemyCount_excludeBoss > 0) {
                    return;
                }

                stateModel.SetIsSpawningPaused(false);
                TDLog.Warning($"关卡敌人生成继续,剩余敌人数量: {stateModel.totalSpawnCount - stateModel.curSpawnedCount}");
            }

            // 关卡 是否为重复加载
            if (stateModel.IsRespawning) {
                var roleDomain = worldContext.RootDomain.RoleDomain;
                roleRepo.Foreach_AI(fieldTypeID, ((ai) => {
                    ai.Reset();
                    roleDomain.Show(ai);
                    ai.HudSlotCom.ShowHUD();
                    ai.SetLogicPos(ai.BornPos);
                }));
                fsm.Enter_Finished();
                return;
            }

            // 关卡实体生成
            var curFrame = stateModel.curFrame;
            bool hasBreakPoint = false;
            var entitySpawnCtrlModelArray = field.EntitySpawnCtrlModelArray;
            var len = entitySpawnCtrlModelArray?.Length;

            for (int i = 0; i < len; i++) {
                var entitySpawnCtrlModel = entitySpawnCtrlModelArray[i];

                if (entitySpawnCtrlModel.spawnFrame == curFrame) {
                    if (!stateModel.IsRespawning) {
                        var worldDomain = worldContext.RootDomain;
                        worldDomain.SpawnBy_EntitySpawnCtrlModel(entitySpawnCtrlModel, curFieldTypeID);
                    } else {
                        TDLog.Warning("未处理 关卡实体生成 -- 重复加载关卡");
                    }

                    if (entitySpawnCtrlModel.isBreakPoint) hasBreakPoint = true;

                    stateModel.curSpawnedCount++;
                    stateModel.aliveEnemyCount++;
                }
            }

            // 检查实体是否全部生成完毕
            bool hasSpawnedAll = stateModel.curSpawnedCount >= stateModel.totalSpawnCount;
            bool hasAliveEnemy = stateModel.aliveEnemyCount > 0;
            if (hasSpawnedAll && !hasAliveEnemy) {
                TDLog.Log($"关卡实体生成完毕,总数: {stateModel.totalSpawnCount}");
                fsm.Enter_Finished();
                return;
            }

            stateModel.curFrame++;
            if (hasBreakPoint) {
                stateModel.SetIsSpawningPaused(true);
                TDLog.Warning($"关卡实体生成暂停,请杀死当前所有敌人以推进关卡进度 {stateModel.IsSpawningPaused}");
            }
        }

        void ApplyFSMState_Finished(FieldFSMComponent fsm, float dt) {

        }

    }
}
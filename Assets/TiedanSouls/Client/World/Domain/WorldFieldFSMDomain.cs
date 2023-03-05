using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World.Domain {

    public class WorldFieldFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldFieldFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public void TickFSM_CurrentField(float dt) {
            var stateEntity = worldContext.StateEntity;
            var curFieldTypeID = stateEntity.CurFieldTypeID;
            var fieldRepo = worldContext.FieldRepo;
            if (!fieldRepo.TryGet(curFieldTypeID, out var field)) {
                TDLog.Log($"关卡不存在!: {curFieldTypeID}");
            }

            var fsm = field.FSMComponent;
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

                var door = readyModel.EnterDoorModel;
                var doorPos = door.pos;
                var playerRole = worldContext.RoleRepo.PlayerRole;
                doorPos.y = playerRole.GetPos_Logic().y;
                playerRole.SetPos_Logic(doorPos);
                playerRole.SyncRenderer();
            }

            // TODO: 触发生成敌人的前置条件 如 玩家进入某个区域 或者 玩家点击某个按钮 或者 玩家等待一段时间 或者 对话结束......
            var totalSpawnCount = field.SpawnModelArray.Length;
            fsm.Enter_Spawning(totalSpawnCount);
        }

        void ApplyFSMState_Spawning(FieldEntity field, float dt) {
            var fsm = field.FSMComponent;
            var stateModel = fsm.SpawningModel;
            var roleRepo = worldContext.RoleRepo;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);

                // 判断前置条件: 是否是重复加载关卡
                var fieldTypeID = field.TypeID;
                if (roleRepo.HasFieldRole(fieldTypeID)) {
                    roleRepo.ResetAllAIRolesInField(fieldTypeID, false);
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
                            var itemEntity = worldContext.WorldFactory.SpawnItemEntity(typeID, itemSpawnPos, field.TypeID);
                            TDLog.Log($"物件: EntityID: {itemEntity.EntityD} / TypeID {itemEntity.TypeID} / ItemType {itemEntity.ItemType} / TypeIDForPickUp {itemEntity.TypeIDForPickUp}");
                        }
                        TDLog.Log($"大厅物件生成结束 -------------------------------------------- ");
                    }
                }

            }

            // 刷新当前存活敌人数量
            int aliveEnemyCount = 0;
            roleRepo.ForeachAll_EnemyOfPlayer((enemy) => {
                if (!enemy.AttrCom.IsDead()) {
                    aliveEnemyCount++;
                }
            });
            stateModel.aliveEnemyCount = aliveEnemyCount;

            // 杀死当前所有敌人才能推进关卡进度
            if (stateModel.IsSpawningPaused) {
                if (aliveEnemyCount > 0) {
                    return;
                } else {
                    stateModel.SetIsSpawningPaused(false);
                    TDLog.Warning($"关卡敌人生成继续,剩余敌人数量: {stateModel.totalSpawnCount - stateModel.curSpawnedCount}");
                }
            }

            // 关卡实体生成
            var curFrame = stateModel.curFrame;
            bool hasBreakPoint = false;
            if (stateModel.IsRespawning) {
                TDLog.Warning($"TODO: 如果有关卡来回切换重复加载的情况,就需要在这里处理关卡逻辑重复加载");
            } else {
                var spawnArray = field.SpawnModelArray;
                var len = spawnArray.Length;
                for (int i = 0; i < len; i++) {
                    var spawnModel = spawnArray[i];
                    if (spawnModel.spawnFrame == curFrame) {
                        var worldDomain = worldContext.WorldDomain;
                        worldDomain.SpawnByModel(spawnModel);
                        if (spawnModel.isBreakPoint) {
                            hasBreakPoint = true;
                        }

                        stateModel.curSpawnedCount++;
                        stateModel.aliveEnemyCount++;
                    }
                }
            }

            // 检查实体是否全部生成完毕
            bool hasSpawnedAll = stateModel.curSpawnedCount >= stateModel.totalSpawnCount;
            bool hasAliveEnemy = stateModel.aliveEnemyCount > 0;
            if (hasSpawnedAll && !hasAliveEnemy) {
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
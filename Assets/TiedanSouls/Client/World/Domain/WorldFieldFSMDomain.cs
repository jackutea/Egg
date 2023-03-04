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

        public void TickFSM(int typeID, float dt) {
            var fieldRepo = worldContext.FieldRepo;
            if (!fieldRepo.TryGet(typeID, out var field)) {
                TDLog.Log($"场景不存在! FieldTypeID: {typeID}");
            }

            var fsm = field.FSMComponent;
            var state = fsm.State;
            if (state == FieldFSMState.Ready) {
                ApplyFSMState_Ready(fsm, dt);
            } else if (state == FieldFSMState.Spawning) {
                ApplyFSMState_Spawning(field, dt);
            } else if (state == FieldFSMState.Finished) {
                ApplyFSMState_Finished(fsm, dt);
            }
        }

        void ApplyFSMState_Ready(FieldFSMComponent fsm, float dt) {
            // TODO: 触发生成敌人的前置条件 如 玩家进入某个区域 或者 玩家点击某个按钮 或者 玩家等待一段时间 或者 对话结束......
            fsm.Enter_Spawning();
        }

        void ApplyFSMState_Spawning(FieldEntity field, float dt) {
            var fsm = field.FSMComponent;
            var spawningModel = fsm.SpawningModel;
            if (spawningModel.IsEntering) {
                spawningModel.SetIsEntering(false);

                // 判断是否是重复加载场景
                var roleRepo = worldContext.RoleRepo;
                var fieldTypeID = field.TypeID;
                if (roleRepo.HasFieldRole(fieldTypeID)) {
                    roleRepo.ResetAllAIRolesInField(fieldTypeID, false);
                    spawningModel.isRespawning = true;
                }
            }

            var curFrame = spawningModel.curFrame;

            if (!spawningModel.isRespawning) {
                // 第一次生成
                var spawnArray = field.SpawnModelArray;
                var len = spawnArray.Length;
                for (int i = 0; i < len; i++) {
                    var spawnModel = spawnArray[i];
                    if (spawnModel.spawnFrame == curFrame) {

                        var worldDomain = worldContext.WorldDomain;
                        worldDomain.SpawnByModel(spawnModel);
                    }
                }
            } else {
                TDLog.Warning($"TODO: 如果有场景来回切换重复加载的情况,就需要在这里处理场景逻辑重复加载");
            }

            spawningModel.curFrame++;
        }

        void ApplyFSMState_Finished(FieldFSMComponent fsm, float dt) {

        }

    }
}
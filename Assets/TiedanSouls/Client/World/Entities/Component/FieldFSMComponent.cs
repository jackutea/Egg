using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class FieldFSMComponent {

        FieldFSMState state;
        public FieldFSMState State => state;

        // ====== FSM Model ======
        FieldFSMModel_Ready readyModel;
        public FieldFSMModel_Ready ReadyModel => readyModel;

        FieldFSMModel_Spawning spawningModel;
        public FieldFSMModel_Spawning SpawningModel => spawningModel;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool isExiting) => this.isExiting = isExiting;

        public FieldFSMComponent() {
            state = FieldFSMState.None;
            readyModel = new FieldFSMModel_Ready();
            spawningModel = new FieldFSMModel_Spawning();
        }

        public void Reset() {
            state = FieldFSMState.None;
            readyModel.Reset();
            spawningModel.Reset();
        }

        public void Enter_Ready(FieldDoorModel enterDoorModel) {
            state = FieldFSMState.Ready;
            readyModel.Reset();
            readyModel.SetIsEntering(true);
            readyModel.SetEnterDoorModel(enterDoorModel);
            TDLog.Log($"关卡状态: '{state}'");
        }

        public void Enter_Spawning(int totalSpawnCount) {
            state = FieldFSMState.Spawning;
            spawningModel.Reset();
            spawningModel.SetIsEntering(true);
            spawningModel.totalSpawnCount = totalSpawnCount;
            spawningModel.curSpawnedCount = 0;
            TDLog.Log($"关卡状态: '{state}' 所需生成实体数量: {totalSpawnCount}");
        }

        public void Enter_Finished() {
            state = FieldFSMState.Finished;
            TDLog.Log($"关卡状态: '{state}'");
        }

    }

}
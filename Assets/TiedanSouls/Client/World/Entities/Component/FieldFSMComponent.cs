using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class FieldFSMComponent {

        FieldFSMState state;
        public FieldFSMState State => state;

        // ====== FSM Model ======
        FieldStateModel_Ready readyModel;
        public FieldStateModel_Ready ReadyModel => readyModel;

        FieldStateModel_Spawning spawningModel;
        public FieldStateModel_Spawning SpawningModel => spawningModel;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool isExiting) => this.isExiting = isExiting;

        public FieldFSMComponent() {
            state = FieldFSMState.None;
            readyModel = new FieldStateModel_Ready();
            spawningModel = new FieldStateModel_Spawning();
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
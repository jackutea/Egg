using TiedanSouls.Generic;

namespace TiedanSouls.World.Entities {

    public class FieldFSMComponent {

        FieldFSMState state;
        public FieldFSMState State => state;

        // ====== FSM Model ======
        FieldFSMModel_Ready readyModel;
        public FieldFSMModel_Ready ReadyModel => readyModel;

        FieldFSMModel_Spawning spawningModel;
        public FieldFSMModel_Spawning SpawningModel => spawningModel;

        public FieldFSMComponent() {
            state = FieldFSMState.None;
            readyModel = new FieldFSMModel_Ready();
            spawningModel = new FieldFSMModel_Spawning();
        }

        public void Enter_Ready(FieldDoorModel enterDoorModel) {
            state = FieldFSMState.Ready;
            readyModel.Reset();
            readyModel.SetIsEntering(true);
            readyModel.SetEnterDoorModel(enterDoorModel);
            TDLog.Log($"场景状态: '{state}'");
        }

        public void Enter_Spawning() {
            state = FieldFSMState.Spawning;
            spawningModel.Reset();
            spawningModel.SetIsEntering(true);
            TDLog.Log($"场景状态: '{state}'");
        }

        public void Enter_Finished() {
            state = FieldFSMState.Finished;
            TDLog.Log($"场景状态: '{state}'");
        }

    }

}
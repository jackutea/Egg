using TiedanSouls.Generic;

namespace TiedanSouls.World.Entities {

    public class FieldFSMComponent {

        FieldFSMState state;
        public FieldFSMState State => state;

        // ====== FSM Model ======
        FieldFSMModel_Spawning spawningModel;
        public FieldFSMModel_Spawning SpawningModel => spawningModel;

        public FieldFSMComponent() {
            state = FieldFSMState.None;
            spawningModel = new FieldFSMModel_Spawning();
        }

        public void Enter_Ready() {
            state = FieldFSMState.Ready;
            TDLog.Log($"场景状态: '{state}'");
        }

        public void Enter_Spawning() {
            state = FieldFSMState.Spawning;
            spawningModel.SetIsEntering(true);
            TDLog.Log($"场景状态: '{state}'");
        }

        public void Enter_Finished() {
            state = FieldFSMState.Finished;
            TDLog.Log($"场景状态: '{state}'");
        }

    }

}
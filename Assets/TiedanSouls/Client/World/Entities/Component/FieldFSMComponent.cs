using UnityEngine;


namespace TiedanSouls.World.Entities {

    public class FieldFSMComponent {

        FieldFSMState state;
        public FieldFSMState State => state;

        public FieldFSMComponent() {
            state = FieldFSMState.None;
        }

        public void Enter_Ready() {
            state = FieldFSMState.Ready;
            TDLog.Log($"场景状态: '{state}'");
        }

        public void Enter_Spawning() {
            state = FieldFSMState.Spawning;
            TDLog.Log($"场景状态: '{state}'");
        }

        public void Enter_Finished() {
            state = FieldFSMState.Finished;
            TDLog.Log($"场景状态: '{state}'");
        }

    }

}
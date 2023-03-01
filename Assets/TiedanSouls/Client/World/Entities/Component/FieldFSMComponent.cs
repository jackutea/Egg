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
        }

        public void Enter_Spawning() {
            state = FieldFSMState.Spawning;
        }

        public void Enter_Finished() {
            state = FieldFSMState.Finished;
        }

    }

}
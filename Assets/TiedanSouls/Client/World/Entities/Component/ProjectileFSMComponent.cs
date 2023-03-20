using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class ProjectileFSMComponent {

        ProjectileFSMState state;
        public ProjectileFSMState State => state;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool value) => isExiting = value;

        public ProjectileFSMComponent() { }

        public void Reset() {
            isExiting = false;
            state = ProjectileFSMState.None;
        }

        public void Enter_Deactivated() {
            state = ProjectileFSMState.Deactivated;
        }

        public void Enter_Activated() {
            state = ProjectileFSMState.Activated;
        }

        public void Enter_Destroyed() {
            state = ProjectileFSMState.Destroyed;
        }

    }
}
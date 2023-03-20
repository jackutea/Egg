using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class ProjectileElementFSMComponent {

        ProjectileElementFSMState state;
        public ProjectileElementFSMState State => state;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool value) => isExiting = value;

        public ProjectileElementFSMComponent() { }

        public void Reset() {
            isExiting = false;
            state = ProjectileElementFSMState.None;
        }

        public void Enter_Deactivated() {
            state = ProjectileElementFSMState.Deactivated;
        }

        public void Enter_Activated() {
            state = ProjectileElementFSMState.Activated;
        }

        public void Enter_Destroyed() {
            state = ProjectileElementFSMState.Destroyed;
        }

    }
}
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class ProjectileElementFSMComponent {

        ProjectileElementFSMState state;
        public ProjectileElementFSMState State => state;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool value) => isExiting = value;

        // Model
        ProjectileElementFSMModel_Activated activatedModel;
        public ProjectileElementFSMModel_Activated ActivatedModel => activatedModel;

        ProjectileElementFSMModel_Deactivated deactivatedModel;
        public ProjectileElementFSMModel_Deactivated DeactivatedModel => deactivatedModel;

        ProjectileElementFSMModel_Dying dyingModel;
        public ProjectileElementFSMModel_Dying DyingModel => dyingModel;

        public ProjectileElementFSMComponent() {
            isExiting = false;
            activatedModel = new ProjectileElementFSMModel_Activated();
            deactivatedModel = new ProjectileElementFSMModel_Deactivated();
            dyingModel = new ProjectileElementFSMModel_Dying();
        }

        public void Reset() {
            isExiting = false;
            state = ProjectileElementFSMState.None;
        }

        public void Enter_Deactivated() {
            state = ProjectileElementFSMState.Deactivated;

            deactivatedModel.SetIsEntering(true);
        }

        public void Enter_Activated() {
            state = ProjectileElementFSMState.Activated;

            activatedModel.SetIsEntering(true);
        }

        public void Enter_Destroyed() {
            state = ProjectileElementFSMState.Dying;

            dyingModel.SetIsEntering(true);
        }

    }
}
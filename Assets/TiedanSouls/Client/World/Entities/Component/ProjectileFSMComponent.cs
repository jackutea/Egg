using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class ProjectileFSMComponent {

        ProjectileFSMState state;
        public ProjectileFSMState State => state;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool value) => isExiting = value;

        // Model
        ProjectileFSMModel_Activated activatedModel;
        public ProjectileFSMModel_Activated ActivatedModel => activatedModel;

        ProjectileFSMModel_Deactivated deactivatedModel;
        public ProjectileFSMModel_Deactivated DeactivatedModel => deactivatedModel;

        ProjectileFSMModel_Dying dyingModel;
        public ProjectileFSMModel_Dying DyingModel => dyingModel;

        public ProjectileFSMComponent() {
            isExiting = false;
            activatedModel = new ProjectileFSMModel_Activated();
            deactivatedModel = new ProjectileFSMModel_Deactivated();
            dyingModel = new ProjectileFSMModel_Dying();
        }

        public void Reset() {
            isExiting = false;
            state = ProjectileFSMState.Deactivated;
        }

        public void Enter_Deactivated() {
            state = ProjectileFSMState.Deactivated;

            deactivatedModel.SetIsEntering(true);
            TDLog.Log($"弹道 -状态机 - 进入 Deactivated");
        }

        public void Enter_Activated() {
            state = ProjectileFSMState.Activated;

            activatedModel.SetIsEntering(true);
            TDLog.Log($"弹道 -状态机 - 进入 Activated");
        }

        public void Enter_Destroyed() {
            state = ProjectileFSMState.Dying;

            dyingModel.SetIsEntering(true);
            TDLog.Log($"弹道 -状态机 - 进入 Destroyed");
        }

    }
}
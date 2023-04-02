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
            var stateModel = deactivatedModel;
            stateModel.Reset();
            state = ProjectileFSMState.Deactivated;
            TDLog.Log($"弹道 -状态机 - 进入 {state}");
        }

        public void Enter_Activated() {
            var stateModel = activatedModel;
            stateModel.Reset();
            state = ProjectileFSMState.Activated;
            TDLog.Log($"弹道 -状态机 - 进入 {state}");
        }

        public void Enter_Dying(int maintainFrame) {
            var stateModel = dyingModel;
            stateModel.Reset();
            state = ProjectileFSMState.Dying;
            TDLog.Log($"弹道 -状态机 - 进入 {state}");
        }

        public void Enter_None() {
            state = ProjectileFSMState.None;
            TDLog.Log($"弹道 -状态机 - 进入 {state}");
        }

    }
}
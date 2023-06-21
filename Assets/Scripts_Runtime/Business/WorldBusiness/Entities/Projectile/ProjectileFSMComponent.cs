using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class ProjectileFSMComponent {

        ProjectileFSMState state;
        public ProjectileFSMState State => state;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool value) => isExiting = value;

        // Model
        ProjectileStateModel_Activated activatedModel;
        public ProjectileStateModel_Activated ActivatedModel => activatedModel;

        ProjectileStateModel_Deactivated deactivatedModel;
        public ProjectileStateModel_Deactivated DeactivatedModel => deactivatedModel;

        ProjectileStateModel_Dying dyingModel;
        public ProjectileStateModel_Dying DyingModel => dyingModel;

        public ProjectileFSMComponent() {
            isExiting = false;
            activatedModel = new ProjectileStateModel_Activated();
            deactivatedModel = new ProjectileStateModel_Deactivated();
            dyingModel = new ProjectileStateModel_Dying();
        }

        public void Reset() {
            isExiting = false;
            state = ProjectileFSMState.Deactivated;
        }

        public void Enter_Deactivated() {
            var stateModel = deactivatedModel;
            stateModel.Reset();
            state = ProjectileFSMState.Deactivated;
            TDLog.Log($"弹幕 -状态机 - 进入 {state}");
        }

        public void Enter_Activated() {
            var stateModel = activatedModel;
            stateModel.Reset();
            state = ProjectileFSMState.Activated;
            TDLog.Log($"弹幕 -状态机 - 进入 {state}");
        }

        public void Enter_Dying(int maintainFrame) {
            var stateModel = dyingModel;
            stateModel.Reset();
            state = ProjectileFSMState.Dying;
            TDLog.Log($"弹幕 -状态机 - 进入 {state}");
        }

        public void Enter_None() {
            state = ProjectileFSMState.None;
            TDLog.Log($"弹幕 -状态机 - 进入 {state}");
        }

    }
}
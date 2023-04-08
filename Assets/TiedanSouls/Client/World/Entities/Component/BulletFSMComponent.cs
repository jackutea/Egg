using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class BulletFSMComponent {

        BulletFSMState state;
        public BulletFSMState State => state;

        // Model
        BulletStateModel_Activated activatedModel;
        public BulletStateModel_Activated ActivatedModel => activatedModel;

        BulletStateModel_Deactivated deactivatedModel;
        public BulletStateModel_Deactivated DeactivatedModel => deactivatedModel;

        BulletStateModel_Dying dyingModel;
        public BulletStateModel_Dying TearDownModel => dyingModel;

        public BulletFSMComponent() {
            activatedModel = new BulletStateModel_Activated();
            deactivatedModel = new BulletStateModel_Deactivated();
            dyingModel = new BulletStateModel_Dying();
            state = BulletFSMState.Deactivated;
        }

        public void Reset() {
            state = BulletFSMState.Deactivated;
            activatedModel.Reset();
            deactivatedModel.Reset();
            dyingModel.Reset();
        }

        public void Enter_Deactivated() {
            TDLog.Log($"子弹 -状态机 - 进入 Deactivated");
            state = BulletFSMState.Deactivated;
            deactivatedModel.SetIsEntering(true);
        }

        public void Enter_Activated() {
            TDLog.Log($"子弹 -状态机 - 进入 Activated");
            state = BulletFSMState.Activated;
            activatedModel.SetIsEntering(true);
        }

        public void Enter_Dying(int maintainFrame) {
            TDLog.Log($"子弹 -状态机 - 进入 TearDown");
            state = BulletFSMState.Dying;
            dyingModel.SetIsEntering(true);

            dyingModel.maintainFrame = maintainFrame;
        }

        public void Enter_None() {
            TDLog.Log($"子弹 -状态机 - 进入 None");
            state = BulletFSMState.None;
        }

    }
}
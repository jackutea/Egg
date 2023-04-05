using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class BulletFSMComponent {

        BulletFSMState state;
        public BulletFSMState State => state;

        // Model
        BulletFSMModel_Activated activatedModel;
        public BulletFSMModel_Activated ActivatedModel => activatedModel;

        BulletFSMModel_Deactivated deactivatedModel;
        public BulletFSMModel_Deactivated DeactivatedModel => deactivatedModel;

        BulletFSMModel_Dying dyingModel;
        public BulletFSMModel_Dying TearDownModel => dyingModel;

        public BulletFSMComponent() {
            activatedModel = new BulletFSMModel_Activated();
            deactivatedModel = new BulletFSMModel_Deactivated();
            dyingModel = new BulletFSMModel_Dying();
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
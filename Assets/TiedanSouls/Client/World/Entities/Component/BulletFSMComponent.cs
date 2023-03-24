using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class BulletFSMComponent {

        BulletFSMState state;
        public BulletFSMState State => state;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool value) => isExiting = value;

        // Model
        BulletFSMModel_Activated activatedModel;
        public BulletFSMModel_Activated ActivatedModel => activatedModel;

        BulletFSMModel_Deactivated deactivatedModel;
        public BulletFSMModel_Deactivated DeactivatedModel => deactivatedModel;

        BulletFSMModel_Dying dyingModel;
        public BulletFSMModel_Dying DyingModel => dyingModel;

        public BulletFSMComponent() {
            isExiting = false;
            activatedModel = new BulletFSMModel_Activated();
            deactivatedModel = new BulletFSMModel_Deactivated();
            dyingModel = new BulletFSMModel_Dying();
        }

        public void Reset() {
            isExiting = false;
            state = BulletFSMState.Deactivated;
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

        public void Enter_Destroyed() {
            TDLog.Log($"子弹 -状态机 - 进入 Destroyed");
            state = BulletFSMState.Dying;
            dyingModel.SetIsEntering(true);
        }

    }
}
namespace TiedanSouls.Client.Entities {

    public class RoleDyingStateModel {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int maintainFrame;

        public RoleDyingStateModel() { 
            Reset();
        }

        public void Reset() {
            isEntering = false;
            maintainFrame = 0;
        }

    }

}
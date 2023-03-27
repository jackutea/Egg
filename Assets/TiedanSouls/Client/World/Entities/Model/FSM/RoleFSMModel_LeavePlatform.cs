namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_LeavePlatform {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleFSMModel_LeavePlatform() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
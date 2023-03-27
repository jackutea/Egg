namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_StandInPlatform {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleFSMModel_StandInPlatform() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
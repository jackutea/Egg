namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_StandInCrossPlatform {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleFSMModel_StandInCrossPlatform() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
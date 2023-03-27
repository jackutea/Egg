namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_StandInWater {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleFSMModel_StandInWater() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
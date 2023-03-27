namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_StandInGround {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleFSMModel_StandInGround() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_OnGround {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleFSMModel_OnGround() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
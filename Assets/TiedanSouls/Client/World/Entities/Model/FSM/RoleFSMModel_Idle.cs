namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_Idle {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleFSMModel_Idle() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
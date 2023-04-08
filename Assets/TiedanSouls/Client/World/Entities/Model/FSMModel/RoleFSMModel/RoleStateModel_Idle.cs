namespace TiedanSouls.Client.Entities {

    public class RoleStateModel_Idle {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleStateModel_Idle() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
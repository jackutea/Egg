namespace TiedanSouls.Client.Entities {

    public class RoleStateModel_OnGround {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleStateModel_OnGround() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
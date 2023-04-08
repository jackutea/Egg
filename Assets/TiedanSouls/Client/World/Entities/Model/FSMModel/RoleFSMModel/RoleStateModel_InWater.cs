namespace TiedanSouls.Client.Entities {

    public class RoleStateModel_InWater {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleStateModel_InWater() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
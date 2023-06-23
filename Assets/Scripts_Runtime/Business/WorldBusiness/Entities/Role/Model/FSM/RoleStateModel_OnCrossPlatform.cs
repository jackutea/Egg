namespace TiedanSouls.Client.Entities {

    public class RoleStateModel_OnCrossPlatform {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleStateModel_OnCrossPlatform() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
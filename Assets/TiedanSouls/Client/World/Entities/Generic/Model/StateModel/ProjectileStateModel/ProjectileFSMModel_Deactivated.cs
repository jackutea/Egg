namespace TiedanSouls.Client.Entities {

    public class ProjectileStateModel_Deactivated {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public ProjectileStateModel_Deactivated() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
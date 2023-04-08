namespace TiedanSouls.Client.Entities {

    public class ProjectileStateModel_Activated {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public ProjectileStateModel_Activated() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
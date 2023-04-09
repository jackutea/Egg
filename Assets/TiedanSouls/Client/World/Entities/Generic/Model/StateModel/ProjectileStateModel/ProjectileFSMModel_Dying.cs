namespace TiedanSouls.Client.Entities {

    public class ProjectileStateModel_Dying {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public ProjectileStateModel_Dying() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
namespace TiedanSouls.Client.Entities {

    public class RoleStateModel_Dying {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int maintainFrame;

        public RoleStateModel_Dying() { 
            Reset();
        }

        public void Reset() {
            isEntering = false;
            maintainFrame = 0;
        }

    }

}
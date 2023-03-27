namespace TiedanSouls.Client.Entities {

    public class ProjectileFSMModel_Deactivated {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public ProjectileFSMModel_Deactivated() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_LeaveWater {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleFSMModel_LeaveWater() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
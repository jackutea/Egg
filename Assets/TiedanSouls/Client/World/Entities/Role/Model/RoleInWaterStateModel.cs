namespace TiedanSouls.Client.Entities {

    public class RoleInWaterStateModel {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleInWaterStateModel() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
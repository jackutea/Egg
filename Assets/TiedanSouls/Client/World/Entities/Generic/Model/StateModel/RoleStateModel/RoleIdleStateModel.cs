namespace TiedanSouls.Client.Entities {

    public class RoleIdleStateModel {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleIdleStateModel() { }

        public void Reset() {
            isEntering = false;
        }

    }
}
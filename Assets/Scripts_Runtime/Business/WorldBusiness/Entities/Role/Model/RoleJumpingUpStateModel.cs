namespace TiedanSouls.Client.Entities {

    public class RoleJumpingUpStateModel {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int curFrame;

        public RoleJumpingUpStateModel() {
            Reset();
        }

        public void Reset() {
            isEntering = false;
            curFrame = -1;
        }

    }

}
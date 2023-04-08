namespace TiedanSouls.Client.Entities {

    public class RoleStateModel_JumpingUp {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int curFrame;

        public RoleStateModel_JumpingUp() {
            Reset();
        }

        public void Reset() {
            isEntering = false;
            curFrame = -1;
        }

    }

}
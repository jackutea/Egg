namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_JumpingUp {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int curFrame;

        public RoleFSMModel_JumpingUp() {
            Reset();
        }

        public void Reset() {
            isEntering = false;
            curFrame = -1;
        }

    }

}
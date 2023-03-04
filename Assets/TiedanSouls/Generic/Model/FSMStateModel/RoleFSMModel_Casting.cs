namespace TiedanSouls.Generic {

    public class RoleFSMModel_Casting {

        public int skillorTypeID;
        public bool isCombo;

        public int maintainFrame;

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleFSMModel_Casting() { }

    }
}
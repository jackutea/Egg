namespace TiedanSouls.Generic {

    public class RoleFSMModel_Casting {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        bool isCombo;
        public bool IsCombo => isCombo;
        public void SetIsCombo(bool value) => isCombo = value;

        public int castingSkillTypeID;
        public int maintainFrame;

        public RoleFSMModel_Casting() { }

        public void Reset() {
            isEntering = false;
            isCombo = false;
            castingSkillTypeID = -1;
            maintainFrame = 0;
        }

    }
}
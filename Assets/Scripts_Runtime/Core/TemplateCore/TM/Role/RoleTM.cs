using System;

namespace TiedanSouls.Template {

    [Serializable]
    public class RoleTM {

        // - Identity
        public int typeID;
        public string roleName;

        // - Health
        public int hpMax_Expanded;
        public int gpMax_Expanded;
        public int mpMax_Expanded;

        // - Move
        public int moveSpeed_Expanded;
        public int jumpSpeed_Expanded;
        public int fallSpeed_Expanded;
        public int fallSpeedMax_Expanded;

        // - Skill
        public int[] skillTypeIDArray;

        // - Renderer
        public string modName;

    }

}
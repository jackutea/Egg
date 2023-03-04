using System;

namespace TiedanSouls.Template {

    [Serializable]
    public class RoleTM {

        // - Identity
        public int typeID;
        public string roleName;

        // - Health
        public int hpMax;
        public int gpMax;
        public int epMax;

        // - Move
        public float moveSpeed;
        public float jumpSpeed;
        public float fallingAcceleration;
        public float fallingSpeedMax;

        // - Renderer
        public string modName;

    }

}
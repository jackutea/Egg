using System;

namespace TiedanSouls.Template {

    [Serializable]
    public class RoleTM {

        // - Identity
        public int typeID;
        public string name;

        // - Move
        public float moveSpeed;
        public float jumpSpeed;
        public float fallingAcceleration;
        public float fallingSpeedMax;

        // - Skillor
        public int[] skillorTypeIDArray;

        // - Renderer
        public string meshName;

    }

}
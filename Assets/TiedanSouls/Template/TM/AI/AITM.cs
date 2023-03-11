using System;

namespace TiedanSouls.Template {

    [Serializable]
    public class AITM {
        // - Identity
        public int typeID;
        
        // - AI
        public float sight;
        public float safeDistance;
        public float atkRange;
        public float atkCD;
    }

}
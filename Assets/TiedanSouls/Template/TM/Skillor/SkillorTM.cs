using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillorTM {

        // - Identity
        public int typeID;
        public string name;
        public SkillorType skillorType;

        // - Combo
        public int originalSkillorTypeID;

        // - Frame
        public SkillorFrameTM[] frames;

        // - Renderer
        public string weaponAnimName;
        
        public SkillorTM() { }

    }

}
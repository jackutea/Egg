using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillorTM {

        public int typeID;
        public string name;
        public SkillorType skillorType;

        public SkillorFrameTM[] frames;

        public string weaponAnimName;
        
        public SkillorTM() { }

    }

}
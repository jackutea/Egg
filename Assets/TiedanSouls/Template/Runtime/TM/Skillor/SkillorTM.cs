using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillorTM {

        public int typeID;
        public string name;

        public SkillorFrameTM[] frames;

        public SkillorTM() { }

    }

}
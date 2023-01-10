using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillorFrameTM {

        // - Dash
        public bool hasDash;
        public Vector2 dashForce;
        public bool isDashEnd;

        public SkillorBoxTM[] boxes;

        public SkillorFrameTM() { }

    }

}
using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillorFrameTM {

        // - Hit
        public HitPowerTM hitPower;

        // - Dash
        public bool hasDash;
        public Vector2 dashForce;
        public bool isDashEnd;

        // - Cancel
        public SkillorCancelTM[] cancelTMs;

        public SkillorBoxTM[] boxes;

        public SkillorFrameTM() { }

    }

}
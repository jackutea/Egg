using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class HitPowerTM {

        // ==== Attack ====
        public float knockbackForce;
        public short knockbackFrame;

        public float blowUpForce;

        public short hitStunFrame;

        public short breakPowerLevel;

        // ==== Defense ====
        public short sufferPowerLevel;

        // ==== Other ====
        public short hitStopFrame;

    }

}
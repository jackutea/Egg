using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class HitPowerTM {

        public int startFrame;
        public int endFrame;
        public int[] hitStunFrameArray;
        public int[] damageArray;

        public int[] knockBackVelocityArray_cm;
        public int[] knockUpVelocityArray_cm;

    }

}
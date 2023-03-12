using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class HitPowerTM {

        public int startFrame;
        public int endFrame;
        public float[] damageArray;
        public int[] hitStunFrameArray;

        public Vector2[] knockBackVelocityArray;
        public Vector2[] knockUpVelocityArray;

    }

}
using UnityEngine;

namespace TiedanSouls.Generic {

    public struct HitPowerModel {

        public int startFrame;
        public int endFrame;
        public float[] damageArray;
        public int[] hitStunFrameArray;

        public Vector2[] knockBackVelocityArray;
        public Vector2[] knockUpVelocityArray;

    }

}
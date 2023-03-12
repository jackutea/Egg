using UnityEngine;

namespace TiedanSouls.Generic {

    public struct HitPowerModel {

        public int startFrame;
        public int endFrame;
        public int[] damageArray;
        public int[] hitStunFrameArray;

        public float[] knockBackVelocityArray;
        public float[] knockUpVelocityArray;

    }

}
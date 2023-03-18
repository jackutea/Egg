using UnityEngine;

namespace TiedanSouls.Generic {

    public class RoleFSMModel_BeHit {

        public bool isEntering;
        public int curFrame;

        public Vector3 beHitDir;

        public int knockbackFrame;
        public int hitStunFrame;
        public float[] knockBackVelocityArray;
        public float[] knockUpVelocityArray;

        public RoleFSMModel_BeHit() { }

        public void Reset() {
            isEntering = false;
            curFrame = 0;

            beHitDir = Vector3.zero;

            knockbackFrame = 0;
            hitStunFrame = 0;
            knockBackVelocityArray = null;
            knockUpVelocityArray = null;
        }

    }

}
using UnityEngine;

namespace TiedanSouls.Generic {

    public class RoleFSMModel_BeHit {

        public Vector2 fromPos;

        public float knockbackForce;
        public int knockbackFrame;

        public int hitStunFrame;

        public int curFrame;

        public bool isEntering;

        public RoleFSMModel_BeHit() { }

        public void Reset() {
            fromPos = Vector2.zero;
            knockbackForce = 0;
            knockbackFrame = 0;
            hitStunFrame = 0;
            curFrame = 0;
            isEntering = false;
        }

    }

}
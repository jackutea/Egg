using UnityEngine;

namespace TiedanSouls.Generic {

    public class RoleFSMModel_BeHit {

        public bool isEntering;

        public Vector2 beHitDir;

        public int hitStunFrame;
        public int castingSkillTypeID;

        public float[] knockBackSpeedArray;
        public float[] knockUpSpeedArray;

        public int curFrame;

        public RoleFSMModel_BeHit() { }

        public void Reset() {
            isEntering = false;

            beHitDir = Vector2.zero;

            hitStunFrame = 0;
            castingSkillTypeID = -1;

            knockBackSpeedArray = null;
            knockUpSpeedArray = null;

            curFrame = 0;
        }

    }

}
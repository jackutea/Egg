using UnityEngine;

namespace TiedanSouls.Generic {

    public class RoleFSMModel_BeHit {

        public bool isEntering;

        public Vector3 beHitDir;

        public int hitStunFrame;
        public int castingSkillTypeID;

        public float[] knockBackVelocityArray;
        public float[] knockUpVelocityArray;

        public RoleFSMModel_BeHit() { }

        public void Reset() {
            isEntering = false;

            beHitDir = Vector3.zero;

            hitStunFrame = 0;
            castingSkillTypeID = -1;

            knockBackVelocityArray = null;
            knockUpVelocityArray = null;
        }

    }

}
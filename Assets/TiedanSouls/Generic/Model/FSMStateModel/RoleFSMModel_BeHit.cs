using UnityEngine;

namespace TiedanSouls.Generic {

    public class RoleFSMModel_BeHit {

        public bool isEntering;
        public int curFrame;

        public Vector2 beHitDir;
        public int castingSkillTypeID;

        // 模型来源数据
        public int hitStunFrame;
        public float[] knockBackSpeedArray;
        public float[] knockUpSpeedArray;

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
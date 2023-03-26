using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_KnockBack {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public Vector2 beHitDir;

        public float[] knockBackSpeedArray;
        public int curFrame;

        public RoleFSMModel_KnockBack() {
            Reset();
        }

        public void Reset() {
            isEntering = false;
            beHitDir = Vector2.zero;
            knockBackSpeedArray = null;
            curFrame = -1;
        }

    }

}
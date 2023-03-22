using UnityEngine;

namespace TiedanSouls.Generic {

    public class RoleFSMModel_KnockUp {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int curFrame;

        public float[] knockUpSpeedArray;

        public RoleFSMModel_KnockUp() { }

        public void Reset() {
            isEntering = false;
            knockUpSpeedArray = null;
            curFrame = 0;
        }

    }

}
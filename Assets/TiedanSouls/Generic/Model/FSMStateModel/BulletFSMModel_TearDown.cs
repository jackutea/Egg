using UnityEngine;

namespace TiedanSouls.Generic {

    public class BulletFSMModel_TearDown {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int maintainFrame;

        public BulletFSMModel_TearDown() {
            isEntering = false;
            maintainFrame = 0;
        }

        public void Reset() {
            isEntering = false;
            maintainFrame = 0;
        }

    }

}
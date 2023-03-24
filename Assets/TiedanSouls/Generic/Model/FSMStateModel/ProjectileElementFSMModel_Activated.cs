using UnityEngine;

namespace TiedanSouls.Generic {

    public class BulletFSMModel_Activated {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int curFrame;

        public BulletFSMModel_Activated() { }

        public void Reset() {
            isEntering = false;
            curFrame = -1;
        }

    }

}
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class BulletStateModel_Activated {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int curFrame;

        public BulletStateModel_Activated() { 
            Reset();
        }

        public void Reset() {
            isEntering = false;
            curFrame = -1;
        }

    }

}
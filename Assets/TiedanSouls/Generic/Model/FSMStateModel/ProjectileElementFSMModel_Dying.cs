using UnityEngine;

namespace TiedanSouls.Generic {

    public class BulletFSMModel_Dying {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public BulletFSMModel_Dying() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
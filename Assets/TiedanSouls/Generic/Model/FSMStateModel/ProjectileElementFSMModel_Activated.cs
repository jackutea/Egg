using UnityEngine;

namespace TiedanSouls.Generic {

    public class ProjectileElementFSMModel_Activated {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public ProjectileElementFSMModel_Activated() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
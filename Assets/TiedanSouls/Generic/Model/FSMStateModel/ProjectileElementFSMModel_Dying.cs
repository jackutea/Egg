using UnityEngine;

namespace TiedanSouls.Generic {

    public class ProjectileElementFSMModel_Dying {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public ProjectileElementFSMModel_Dying() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
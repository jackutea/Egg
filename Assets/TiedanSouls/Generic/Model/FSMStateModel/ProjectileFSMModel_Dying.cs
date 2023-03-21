using UnityEngine;

namespace TiedanSouls.Generic {

    public class ProjectileFSMModel_Dying {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public ProjectileFSMModel_Dying() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
using UnityEngine;

namespace TiedanSouls.Generic {

    public class ProjectileFSMModel_Activated {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public ProjectileFSMModel_Activated() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
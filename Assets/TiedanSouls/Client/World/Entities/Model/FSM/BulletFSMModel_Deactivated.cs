using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class BulletFSMModel_Deactivated {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public BulletFSMModel_Deactivated() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
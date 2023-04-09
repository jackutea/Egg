using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class BulletStateModel_Deactivated {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public BulletStateModel_Deactivated() { }

        public void Reset() {
            isEntering = false;
        }

    }

}
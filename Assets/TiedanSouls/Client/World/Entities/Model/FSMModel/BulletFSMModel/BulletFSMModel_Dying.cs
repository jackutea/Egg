using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class BulletStateModel_Dying {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int maintainFrame;

        public BulletStateModel_Dying() {
            isEntering = false;
            maintainFrame = 0;
        }

        public void Reset() {
            isEntering = false;
            maintainFrame = 0;
        }

    }

}
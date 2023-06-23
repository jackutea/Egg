using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RoleStateModel_KnockUp {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public int curFrame;

        public float[] knockUpSpeedArray;

        public RoleStateModel_KnockUp() {
            Reset();
        }

        public void Reset() {
            isEntering = false;
            knockUpSpeedArray = null;
            curFrame = -1;
        }

    }

}
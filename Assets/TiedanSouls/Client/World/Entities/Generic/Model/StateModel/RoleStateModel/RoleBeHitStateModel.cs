using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RoleBeHitStateModel {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        float maintainFrame;
        public float MaintainFrame => maintainFrame;
        public void SetMaintainFrame(float value) => maintainFrame = value;

        float[] knockBackSpeedArray;
        public float[] KnockBackSpeedArray => knockBackSpeedArray;
        public void SetKnockBackSpeedArray(float[] value) => knockBackSpeedArray = value;

        float[] knockUpSpeedArray;
        public float[] KnockUpSpeedArray => knockUpSpeedArray;
        public void SetKnockUpSpeedArray(float[] value) => knockUpSpeedArray = value;

        Vector3 beHitDir;
        public Vector3 BeHitDir => beHitDir;
        public void SetBeHitDir(Vector3 value) => beHitDir = value;

        public int curFrame;

        public RoleBeHitStateModel() { }

        public void Reset() {
            isEntering = false;
            maintainFrame = 0;
            knockBackSpeedArray = null;
            knockUpSpeedArray = null;
            curFrame = -1;
        }

    }
}
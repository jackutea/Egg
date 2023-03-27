using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RoleFSMModel_SkillMove {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        bool isFaceTo;
        public bool IsFaceTo => isFaceTo;
        public void SetIsFaceTo(bool value) => isFaceTo = value;

        float[] moveSpeedArray;
        public float[] MoveSpeedArray => moveSpeedArray;
        public void SetMoveSpeedArray(float[] value) => moveSpeedArray = value;

        Vector3[] moveDirArray;
        public Vector3[] MoveDirArray => moveDirArray;
        public void SetMoveDirArray(Vector3[] value) => moveDirArray = value;

        public int curFrame;

        public RoleFSMModel_SkillMove() {
            Reset();
        }

        public void Reset() {
            isEntering = false;
            isFaceTo = false;
            moveSpeedArray = null;
            moveDirArray = null;
            curFrame = -1;
        }

    }
}
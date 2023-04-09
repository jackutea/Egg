using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RoleStateModel_SkillMove {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        bool isFaceTo;
        public bool IsFaceTo => isFaceTo;
        public void SetIsFaceTo(bool value) => isFaceTo = value;

        bool needWaitForMoveEnd;
        public bool NeedWaitForMoveEnd => needWaitForMoveEnd;
        public void SetNeedWaitForMoveEnd(bool value) => needWaitForMoveEnd = value;

        float[] moveSpeedArray;
        public float[] MoveSpeedArray => moveSpeedArray;
        public void SetMoveSpeedArray(float[] value) => moveSpeedArray = value;

        Vector3[] moveDirArray;
        public Vector3[] MoveDirArray => moveDirArray;
        public void SetMoveDirArray(Vector3[] value) => moveDirArray = value;

        public int curFrame;

        public RoleStateModel_SkillMove() {
            Reset();
        }

        public void Reset() {
            isEntering = false;
            isFaceTo = false;
            needWaitForMoveEnd = false;
            moveSpeedArray = null;
            moveDirArray = null;
            curFrame = -1;
        }

    }
}
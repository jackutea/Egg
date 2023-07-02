using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RoleCastingStateModel {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        SkillEntity castingSkill;
        public SkillEntity CastingSkill => castingSkill;
        public void SetCastingSkill(SkillEntity value) => castingSkill = value;

        Vector2 chosedPoint;
        public Vector2 ChosedPoint => chosedPoint;
        public void SetChosedPoint(Vector2 value) => chosedPoint = value;

        Quaternion casterRotation;
        public Quaternion CasterRotation => casterRotation;
        public void SetCasterRotation(Quaternion value) => casterRotation = value;

        bool isWaitingForMoveEnd;
        public bool IsWaitingForMoveEnd => isWaitingForMoveEnd;
        public void SetIsWaitingForMoveEnd(bool value) => isWaitingForMoveEnd = value;

        short[] frameArray;
        public short[] FrameArray => frameArray;

        int frameCount;
        public int FrameCount => frameCount;
        public void SetFrameCount(int value) => frameCount = value;

        public int curIndex;

        public RoleCastingStateModel() {
            frameArray = new short[1000];
        }

        public void Reset() {
            isEntering = false;
            isWaitingForMoveEnd = false;
            castingSkill = null;
            chosedPoint = Vector2.zero;
            frameCount = 0;
            curIndex = -1;
        }

        public bool IsCurrentValid() {
            return curIndex >= 0 && curIndex < frameCount;
        }

        public int GetCurFrame() {
            if (curIndex < 0 || curIndex >= frameCount) return -1;
            return frameArray[curIndex];
        }

        public int GetLastFrame() {
            var lastIndex = curIndex - 1;
            if (lastIndex < 0 || lastIndex >= frameCount) return 0;
            return frameArray[lastIndex];
        }

    }
}
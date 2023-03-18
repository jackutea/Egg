namespace TiedanSouls.Generic {

    public struct SkillCancelModel {

        public int startFrame;
        public int endFrame;

        public int skillTypeID;

        public bool IsInTriggeringFrame(int frame) {
            return frame >= startFrame && frame <= endFrame;
        }

    }

}
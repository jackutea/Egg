namespace TiedanSouls.Client.Entities {

    public struct SkillCancelModel {

        public int startFrame;
        public int endFrame;
        public SkillCancelType cancelType;
        public int skillTypeID;

        public bool IsInTriggeringFrame(int frame) {
            return frame >= startFrame && frame <= endFrame;
        }

    }

}
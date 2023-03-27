namespace TiedanSouls.Generic {

    /// <summary>
    /// 人物状态标记 (状态标记是一个32位的整数，每一位代表一个状态，可以同时存在多个状态)
    /// </summary>
    [System.Flags]
    public enum RoleStateFlag : int {

        None = 0,               // 无状态
        Idle = 1 << 0,          // 待机
        Dying = 1 << 1,         // 死亡
        Cast = 1 << 2,          // 施法
        SkillMove = 1 << 3,     // 技能移动
        KnockBack = 1 << 4,     // 击退
        KnockUp = 1 << 5,       // 击飞
        Root = 1 << 6,          // 禁锢
        Stun = 1 << 7,          // 眩晕
        Silence = 1 << 8,       // 沉默

        StandInGround = 1 << 20,    // 站在地面上
        LeaveGround = 1 << 21,      // 离开地面

        StandInPlatform = 1 << 22,  // 站在平台上
        LeavePlatform = 1 << 23,    // 离开平台

        StandInWater = 1 << 24,     // 站在水面上
        LeaveWater = 1 << 25,       // 离开水面

    }

    public static class StateTypeExtension {

        public static bool Contains(this RoleStateFlag stateType, RoleStateFlag stateTypeToCheck) {
            return (stateType & stateTypeToCheck) != 0;
        }

        public static RoleStateFlag AddStateFlag(this RoleStateFlag stateType, RoleStateFlag stateTypeToAdd) {
            return stateType | stateTypeToAdd;
        }

        public static RoleStateFlag RemoveStateFlag(this RoleStateFlag stateType, RoleStateFlag stateTypeToRemove) {
            return stateType & ~stateTypeToRemove;
        }

        public static string ToString_AllFlags(this RoleStateFlag stateFlag) {
            if (stateFlag == 0) return "状态标记列表: 无状态";

            string result = "状态标记列表: ";
            if (stateFlag.Contains(RoleStateFlag.Idle)) result += "待机 ";
            if (stateFlag.Contains(RoleStateFlag.Dying)) result += "死亡 ";
            if (stateFlag.Contains(RoleStateFlag.Cast)) result += "施法 ";
            if (stateFlag.Contains(RoleStateFlag.KnockBack)) result += "击退 ";
            if (stateFlag.Contains(RoleStateFlag.KnockUp)) result += "击飞 ";
            if (stateFlag.Contains(RoleStateFlag.Root)) result += "禁锢 ";
            if (stateFlag.Contains(RoleStateFlag.Stun)) result += "眩晕 ";
            if (stateFlag.Contains(RoleStateFlag.Silence)) result += "沉默 ";

            return result;
        }

    }

}
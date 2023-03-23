namespace TiedanSouls.Generic {

    /// <summary>
    /// 状态标记 (状态标记是一个32位的整数，每一位代表一个状态，可以同时存在多个状态)
    /// </summary>
    public enum StateFlag {

        None = 0b0000_0000_0000_0000_0000_0000_0000_0000,       // 无状态
        Idle = 0b0000_0000_0000_0000_0000_0000_0000_0001,       // 待机
        Dying = 0b0000_0000_0000_0000_0000_0000_0000_0010,      // 死亡
        Cast = 0b0000_0000_0000_0000_0000_0000_0000_0100,       // 施法
        KnockBack = 0b0000_0000_0000_0000_0000_0000_0000_1000,  // 击退
        KnockUp = 0b0000_0000_0000_0000_0000_0000_0001_0000,    // 击飞
        Root = 0b0000_0000_0000_0000_0000_0000_0010_0000,       // 禁锢
        Stun = 0b0000_0000_0000_0000_0000_0000_0100_0000,       // 眩晕
        Silence = 0b0000_0000_0000_0000_0000_0000_1000_0000,    // 沉默

    }

    public static class StateTypeExtension {

        public static bool Contains(this StateFlag stateType, StateFlag stateTypeToCheck) {
            return (stateType & stateTypeToCheck) != 0;
        }

        public static StateFlag AddStateFlag(this StateFlag stateType, StateFlag stateTypeToAdd) {
            return stateType | stateTypeToAdd;
        }

        public static StateFlag RemoveStateFlag(this StateFlag stateType, StateFlag stateTypeToRemove) {
            return stateType & ~stateTypeToRemove;
        }

        public static string ToString_AllFlags(this StateFlag stateFlag) {
            if (stateFlag == 0) return "状态标记列表: 无状态";

            string result = "状态标记列表: ";
            if (stateFlag.Contains(StateFlag.Idle)) result += "待机 ";
            if (stateFlag.Contains(StateFlag.Dying)) result += "死亡 ";
            if (stateFlag.Contains(StateFlag.Cast)) result += "施法 ";
            if (stateFlag.Contains(StateFlag.KnockBack)) result += "击退 ";
            if (stateFlag.Contains(StateFlag.KnockUp)) result += "击飞 ";
            if (stateFlag.Contains(StateFlag.Root)) result += "禁锢 ";
            if (stateFlag.Contains(StateFlag.Stun)) result += "眩晕 ";
            if (stateFlag.Contains(StateFlag.Silence)) result += "沉默 ";

            return result;
        }

    }

}
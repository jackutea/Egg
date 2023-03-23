namespace TiedanSouls.Generic {

    /// <summary>
    /// 状态标记 (状态标记是一个32位的整数，每一位代表一个状态，可以同时存在多个状态)
    /// </summary>
    public enum StateFlag : int {

        None = 0,               // 无状态
        Idle = 1 << 0,          // 待机
        Dying = 1 << 1,         // 死亡
        Cast = 1 << 2,          // 施法
        KnockBack = 1 << 3,     // 击退
        KnockUp = 1 << 4,       // 击飞
        Root = 1 << 5,          // 禁锢
        Stun = 1 << 6,          // 眩晕
        Silence = 1 << 7,       // 沉默

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
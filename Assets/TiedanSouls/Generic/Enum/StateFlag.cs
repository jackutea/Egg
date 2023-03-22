namespace TiedanSouls.Generic {

    /// <summary>
    /// 状态标记 (状态标记是一个32位的整数，每一位代表一个状态，可以同时存在多个状态)
    /// </summary>
    public enum StateFlag {

        Idle = 0b0000_0000_0000_0000_0000_0000_0000_0000,           // 待机
        Dying = 0b0000_0000_0000_0000_0000_0000_0000_0001,          // 死亡
        Cast = 0b0000_0000_0000_0000_0000_0000_0000_0010,           // 施法
        KnockBack = 0b0000_0000_0000_0000_0000_0000_0000_0100,      // 击退
        KnockUp = 0b0000_0000_0000_0000_0000_0000_0000_1000,        // 击飞
        KnockDown = 0b0000_0000_0000_0000_0000_0000_0001_0000,      // 击倒
        Root = 0b0000_0000_0000_0000_0000_0000_0000_0010_0000,      // 禁锢
        Stun = 0b0000_0000_0000_0000_0000_0000_0000_0100_0000,      // 眩晕
        Slow = 0b0000_0000_0000_0000_0000_0000_0000_1000_0000,      // 减速
        Silence = 0b0000_0000_0000_0000_0000_0000_0001_0000_0000,   // 沉默
        Disarm = 0b0000_0000_0000_0000_0000_0000_0010_0000_0000,    // 缴械
        Sleep = 0b0000_0000_0000_0000_0000_0000_0100_0000_0000,     // 睡眠
        Fear = 0b0000_0000_0000_0000_0000_0000_1000_0000_0000,      // 恐惧
        Confuse = 0b0000_0000_0000_0000_0000_0001_0000_0000_0000,   // 混乱
        Poison = 0b0000_0000_0000_0000_0000_0010_0000_0000_0000,    // 中毒
        Bleed = 0b0000_0000_0000_0000_0000_0100_0000_0000_0000,     // 流血
        Burn = 0b0000_0000_0000_0000_0000_1000_0000_0000_0000,      // 燃烧
        Charm = 0b0000_0000_0000_0000_0001_0000_0000_0000_0000,     // 魅惑
        Blind = 0b0000_0000_0000_0000_0010_0000_0000_0000_0000,     // 致盲
        Freeze = 0b0000_0000_0000_0000_0100_0000_0000_0000_0000,    // 冰冻
        Shock = 0b0000_0000_0000_0000_1000_0000_0000_0000_0000,     // 震荡

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

        public static string ToString(this StateFlag stateType) {
            string result = "状态标记列表: ";
            if (stateType.Contains(StateFlag.Cast)) result += "施法 ";
            if (stateType.Contains(StateFlag.KnockBack)) result += "击退 ";
            if (stateType.Contains(StateFlag.KnockUp)) result += "击飞 ";
            if (stateType.Contains(StateFlag.KnockDown)) result += "击倒 ";
            if (stateType.Contains(StateFlag.Root)) result += "禁锢 ";
            if (stateType.Contains(StateFlag.Stun)) result += "眩晕 ";
            if (stateType.Contains(StateFlag.Slow)) result += "减速 ";
            if (stateType.Contains(StateFlag.Silence)) result += "沉默 ";
            if (stateType.Contains(StateFlag.Sleep)) result += "睡眠 ";
            if (stateType.Contains(StateFlag.Burn)) result += "燃烧 ";
            if (stateType.Contains(StateFlag.Poison)) result += "中毒 ";
            if (stateType.Contains(StateFlag.Bleed)) result += "出血 ";
            if (stateType.Contains(StateFlag.Freeze)) result += "冰冻 ";
            if (stateType.Contains(StateFlag.Shock)) result += "震荡 ";
            if (stateType.Contains(StateFlag.Fear)) result += "恐惧 ";
            if (stateType.Contains(StateFlag.Confuse)) result += "混乱 ";
            if (stateType.Contains(StateFlag.Charm)) result += "魅惑 ";
            if (stateType.Contains(StateFlag.Blind)) result += "致盲 ";
            return result;
        }

    }

}
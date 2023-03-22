namespace TiedanSouls.Generic {

    /// <summary>
    /// 状态类型，比如禁锢、眩晕、减速等
    /// </summary>
    public enum StateType {

        None = 0b0000_0000_0000_0000_0000_0000_0000_0000,      // 无效果
        Root = 0b0000_0000_0000_0000_0000_0000_0000_0001,      // 禁锢
        Stun = 0b0000_0000_0000_0000_0000_0000_0000_0010,      // 眩晕
        Slow = 0b0000_0000_0000_0000_0000_0000_0000_0100,      // 减速
        Silence = 0b0000_0000_0000_0000_0000_0000_0000_1000,   // 沉默
        Sleep = 0b0000_0000_0000_0000_0000_0000_0001_0000,     // 睡眠
        Burn = 0b0000_0000_0000_0000_0000_0000_0010_0000,      // 燃烧
        Poison = 0b0000_0000_0000_0000_0000_0000_0100_0000,    // 中毒
        Bleed = 0b0000_0000_0000_0000_0000_0000_1000_0000,     // 出血
        Freeze = 0b0000_0000_0000_0000_0000_0001_0000_0000,    // 冰冻
        Shock = 0b0000_0000_0000_0000_0000_0010_0000_0000,     // 震荡
        Fear = 0b0000_0000_0000_0000_0000_0100_0000_0000,      // 恐惧
        Confuse = 0b0000_0000_0000_0000_0000_1000_0000_0000,   // 混乱
        Charm = 0b0000_0000_0000_0000_0001_0000_0000_0000,     // 魅惑
        Blind = 0b0000_0000_0000_0000_0010_0000_0000_0000,     // 致盲

    }

    public static class StateTypeExtension {

        public static bool HasStateType(this StateType stateType, StateType stateTypeToCheck) {
            return (stateType & stateTypeToCheck) != 0;
        }

        public static StateType AddStateType(this StateType stateType, StateType stateTypeToAdd) {
            return stateType | stateTypeToAdd;
        }

        public static StateType RemoveStateType(this StateType stateType, StateType stateTypeToRemove) {
            return stateType & ~stateTypeToRemove;
        }

    }

}
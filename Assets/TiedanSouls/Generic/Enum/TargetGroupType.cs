using System;

namespace TiedanSouls.Generic {

    // 目标群体类型
    [Flags]
    public enum TargetGroupType : sbyte {

        None = 0,

        Self = 1 << 0,                      // 自身
        Ally = 1 << 1,                      // 友军
        Enemy = 1 << 2,                     // 敌军
        Neutral = 1 << 3,                   // 中立

    }

    public static class TargetGroupTypeExtension {

        public static bool Contains(this TargetGroupType self, TargetGroupType other) {
            return (self & other) == other;
        }

        public static TargetGroupType ChooseAll() {
            return (TargetGroupType)sbyte.MaxValue;
        }

    }

}
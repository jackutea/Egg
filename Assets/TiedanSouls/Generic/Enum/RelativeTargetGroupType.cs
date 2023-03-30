using System;

namespace TiedanSouls.Generic {

    // 相对目标群体类型
    [Flags]
    public enum RelativeTargetGroupType {

        None = 0,

        Self = 1 << 0,                      // 自身
        Ally = 1 << 1,                      // 友军
        Enemy = 1 << 2,                     // 敌军
        Neutral = 1 << 3,                   // 中立

    }

    public static class RelativeTargetGroupTypeExtension {

        public static bool Contains(this RelativeTargetGroupType self, RelativeTargetGroupType other) {
            return (self & other) == other;
        }

    }

}
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client {

    public static class MathUtil {

        public static float GetClampOffset(float curValue, float baseValue, float min, float max, NumCalculationType nct) {
            float offset = 0;
            switch (nct) {
                case NumCalculationType.PercentageAdd:
                    offset = baseValue * curValue;
                    break;
                case NumCalculationType.PercentageMul:
                    offset = curValue * baseValue;
                    break;
                case NumCalculationType.AbsoluteAdd:
                    offset = curValue;
                    break;
            }

            var afterV = curValue + offset;
            if (afterV < min) {
                offset = min - curValue;
            } else if (afterV > max) {
                offset = max - curValue;
            }

            return offset;
        }

    }

}
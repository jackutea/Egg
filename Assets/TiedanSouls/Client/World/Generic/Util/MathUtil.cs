using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client {

    public static class MathUtil {

        public static float GetClampOffset(float curValue, float baseValue, float effectValue, float min, float max, NumCalculationType nct) {
            float realOffset = 0;
            switch (nct) {
                case NumCalculationType.PercentageAdd:
                    realOffset = effectValue * baseValue;
                    break;
                case NumCalculationType.PercentageMul:
                    realOffset = effectValue * curValue;
                    break;
                case NumCalculationType.AbsoluteAdd:
                    realOffset = effectValue;
                    break;
                default:
                    break;
            }

            var afterValue = curValue + realOffset;
            if (afterValue < min) {
                realOffset = min - curValue;
            } else if (afterValue > max) {
                realOffset = max - curValue;
            }

            return realOffset;
        }

    }

}
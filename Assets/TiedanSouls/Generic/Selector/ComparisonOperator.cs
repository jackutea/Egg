namespace TiedanSouls.Generic {

    /// <summary>
    /// 对比类型
    /// </summary> 
    public enum ComparisonType {

        None,           // 无
        Less,           // 小于
        LessOrEqual,    // 小于等于
        Equal,          // 等于
        Greater,        // 大于
        GreaterOrEqual, // 大于等于
        NotEqual,       // 不等于

    }

    public static class ComparisonTypeExtension {

        public static bool IsMatch(this ComparisonType comparisonType, int a, int b) {
            if (comparisonType == ComparisonType.None) return true;
            if (comparisonType == ComparisonType.Less) return a < b;
            if (comparisonType == ComparisonType.LessOrEqual) return a <= b;
            if (comparisonType == ComparisonType.Equal) return a == b;
            if (comparisonType == ComparisonType.Greater) return a > b;
            if (comparisonType == ComparisonType.GreaterOrEqual) return a >= b;
            if (comparisonType == ComparisonType.NotEqual) return a != b;
            return false;
        }

        public static bool IsMatch(this ComparisonType comparisonType, float a, float b) {
            if (comparisonType == ComparisonType.None) return true;
            if (comparisonType == ComparisonType.Less) return a < b;
            if (comparisonType == ComparisonType.LessOrEqual) return a <= b;
            if (comparisonType == ComparisonType.Equal) return a == b;
            if (comparisonType == ComparisonType.Greater) return a > b;
            if (comparisonType == ComparisonType.GreaterOrEqual) return a >= b;
            if (comparisonType == ComparisonType.NotEqual) return a != b;
            return false;
        }

    }

}
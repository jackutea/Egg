namespace TiedanSouls {

    public enum AllyType {

        None,
        One,    // 玩家阵营
        Two,    // 敌人阵营
        Neutral,

    }

    public static class AllyTypeExtension {

        public static bool IsAlly(this AllyType self, AllyType other) {
            return self == other;
        }

        public static bool IsEnemy(this AllyType self, AllyType other) {
            return self != other && other != AllyType.Neutral;
        }

        public static bool IsNeutral(this AllyType self, AllyType other) {
            return other == AllyType.Neutral;
        }

    }

}
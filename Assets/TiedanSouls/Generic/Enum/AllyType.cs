namespace TiedanSouls.Generic {

    // 绝对阵营
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
            return self != other && self != AllyType.Neutral && other != AllyType.Neutral;
        }

    }

}
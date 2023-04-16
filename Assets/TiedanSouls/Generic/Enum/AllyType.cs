namespace TiedanSouls.Generic {

    // 绝对阵营 TODO: CampType
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
            if (self == AllyType.Neutral || other == AllyType.Neutral) {
                return false;
            }

            if (self == AllyType.None || other == AllyType.None) {
                return false;
            }

            return self != other;
        }

    }

}
namespace TiedanSouls.Generic {

    // 相对群体类型
    public enum RelativeTargetGroupType {

        None = 0b0000_0000,

        All = 0b1111_1111,
        AllExceptSelf = 0b1111_1110,
        AllExceptAlly = 0b1111_1101,
        AllExceptEnemy = 0b1111_1011,
        AllExceptNeutral = 0b1111_0111,

        OnlySelf = 0b0000_0001,
        OnlyAlly = 0b0000_0010,
        OnlyEnemy = 0b0000_0100,
        OnlyNeutral = 0b0000_1000,

        SelfAndAlly = OnlyAlly | OnlySelf,
        SelfAndEnemy = OnlyEnemy | OnlySelf,
        SelfAndNeutral = OnlyNeutral | OnlySelf,

        AllyAndEnemy = OnlyAlly | OnlyEnemy,
        AllyAndNeutral = OnlyAlly | OnlyNeutral,
        EnemyAndNeutral = OnlyEnemy | OnlyNeutral,

    }

}
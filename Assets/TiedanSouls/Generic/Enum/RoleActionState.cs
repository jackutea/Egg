namespace TiedanSouls.Generic {

    /// <summary>
    /// 人物动作状态
    /// </summary>
    public enum RoleActionState : sbyte {

        None,                   // 无状态
        Idle,                   // 待机
        Moving,                 // 移动
        JumpingUp,              // 上跳
        JumpingDown,            // 下跳
        Falling,                // 下落
        Dying,                  // 死亡
        Casting,                // 施法
        SkillPreparing,         // 技能蓄积中

    }

}
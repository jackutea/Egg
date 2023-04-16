namespace TiedanSouls.Generic {

    /// <summary>
    /// 人物动作状态
    /// </summary>
    public enum RoleFSMState : sbyte {

        None,                   // 无状态
        Idle,                   // 待机
        Moving,                 // 移动
        JumpingUp,              // 上跳
        JumpingDown,            // 下跳
        Falling,                // 下落
        BeHit,                  // 受击
        Dying,                  // 死亡
        Casting,                // 施法

    }

}
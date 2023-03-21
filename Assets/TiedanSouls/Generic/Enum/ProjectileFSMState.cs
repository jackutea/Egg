namespace TiedanSouls.Generic {

    public enum ProjectileFSMState {

        None,               // 弹道 无
        Deactivated,        // 弹道 未激活
        Activated,          // 弹道 激活
        Dying,          // 弹道 被销毁

    }
}
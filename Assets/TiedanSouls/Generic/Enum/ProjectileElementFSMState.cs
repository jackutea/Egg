namespace TiedanSouls.Generic {

    public enum ProjectileElementFSMState {

        None,               // 弹道元素 无
        Deactivated,        // 弹道元素 未激活
        Activated,          // 弹道元素 激活
        Dying,          // 弹道元素 被销毁

    }
}
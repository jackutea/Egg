namespace TiedanSouls.Client.Entities {

    public enum BulletFSMState {

        None,               
        Deactivated,        // 未激活
        Activated,          // 激活
        TearDown,           // 被销毁

    }
}
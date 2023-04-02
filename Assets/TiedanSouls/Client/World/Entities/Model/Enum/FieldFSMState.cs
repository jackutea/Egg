namespace TiedanSouls.Client.Entities {

    public enum FieldFSMState {

        None,   // 无状态
        Ready,  // 玩家进入关卡
        Spawning,  // 玩家一定条件触发关卡,开始生成敌人
        Finished,  // 敌人全部死亡,关卡结束

    }
}
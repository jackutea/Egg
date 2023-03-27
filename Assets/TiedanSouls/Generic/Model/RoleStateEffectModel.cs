namespace TiedanSouls.Generic {

    /// <summary>
    /// 人物状态影响模型，用于描述对人物状态的影响效果，包含所有影响状态的类型
    /// 一般来说，每一种状态都有一个对应的影响模型
    /// </summary>
    public struct RoleStateEffectModel {

        public RoleStateFlag effectStateType;   // 影响状态类型

        public KnockBackModel knockBackModel;   // 击退模型
        public KnockUpModel knockUpModel;       // 击飞模型


    }

}
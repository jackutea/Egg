using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public struct RoleCtrlEffect {

        public RoleCtrlEffectType ctrlEffectType;
        public string iconName;
        public int totalFrame;

        public EntityIDArgs father; // 控制效果由谁施加
        public int curFrame;

        public override string ToString() {
            return $"控制效果类型 {ctrlEffectType} 技能 {father.typeID} 剩余帧数 {totalFrame - curFrame} 总帧数 {totalFrame}\n{father}";
        }

    }

}
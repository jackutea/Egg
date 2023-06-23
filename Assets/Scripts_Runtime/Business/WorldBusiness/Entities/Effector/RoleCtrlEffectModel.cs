using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 控制效果
    /// </summary>
    public struct RoleCtrlEffectModel {

        public RoleCtrlEffectType ctrlEffectType;
        public int totalFrame;
        public string iconName;

        public override string ToString() {
            return $"控制效果类型 {ctrlEffectType} 总帧数 {totalFrame} 图标 {iconName}";
        }

    }

}
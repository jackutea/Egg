namespace TiedanSouls.Generic {

    /// <summary>
    /// 角色控制效果类型
    /// </summary>
    public enum RoleCtrlEffectType : int {

        None = 0,                   // 无状态
        Root = 1 << 0,              // 禁锢
        Stun = 1 << 1,              // 眩晕
        Silence = 1 << 2,           // 沉默

    }

    public static class StateTypeExtension {

        public static bool Contains(this RoleCtrlEffectType stateType, RoleCtrlEffectType stateTypeToCheck) {
            return (stateType & stateTypeToCheck) != 0;
        }

        public static RoleCtrlEffectType AddStatus(this RoleCtrlEffectType stateType, RoleCtrlEffectType stateTypeToAdd) {
            return stateType | stateTypeToAdd;
        }

        public static RoleCtrlEffectType RemoveStatus(this RoleCtrlEffectType stateType, RoleCtrlEffectType stateTypeToRemove) {
            return stateType & ~stateTypeToRemove;
        }

        public static string GetString(this RoleCtrlEffectType ctrlStatus) {
            if (ctrlStatus == 0) return "控制状态列表: 无状态";

            string result = "控制状态列表: ";
            if (ctrlStatus.Contains(RoleCtrlEffectType.Root)) result += "禁锢 ";
            if (ctrlStatus.Contains(RoleCtrlEffectType.Stun)) result += "眩晕 ";
            if (ctrlStatus.Contains(RoleCtrlEffectType.Silence)) result += "沉默 ";

            return result;
        }

    }

}
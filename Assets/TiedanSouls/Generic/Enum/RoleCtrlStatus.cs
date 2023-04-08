namespace TiedanSouls.Generic {

    /// <summary>
    /// 人物控制状态
    /// </summary>
    [System.Flags]
    public enum RoleCtrlStatus : int {

        None = 0,               // 无状态
        SkillMove = 1 << 0,     // 技能移动
        Root = 1 << 3,          // 禁锢
        Stun = 1 << 4,          // 眩晕
        Silence = 1 << 5,       // 沉默
        Rage = 1 << 6,          // 暴怒

    }

    public static class StateTypeExtension {

        public static bool Contains(this RoleCtrlStatus stateType, RoleCtrlStatus stateTypeToCheck) {
            return (stateType & stateTypeToCheck) != 0;
        }

        public static RoleCtrlStatus AddStatus(this RoleCtrlStatus stateType, RoleCtrlStatus stateTypeToAdd) {
            return stateType | stateTypeToAdd;
        }

        public static RoleCtrlStatus RemoveStatus(this RoleCtrlStatus stateType, RoleCtrlStatus stateTypeToRemove) {
            return stateType & ~stateTypeToRemove;
        }

        public static string GetString(this RoleCtrlStatus ctrlStatus) {
            if (ctrlStatus == 0) return "控制状态列表: 无状态";

            string result = "控制状态列表: ";
            if (ctrlStatus.Contains(RoleCtrlStatus.SkillMove)) result += "技能移动 ";
            if (ctrlStatus.Contains(RoleCtrlStatus.Root)) result += "禁锢 ";
            if (ctrlStatus.Contains(RoleCtrlStatus.Stun)) result += "眩晕 ";
            if (ctrlStatus.Contains(RoleCtrlStatus.Silence)) result += "沉默 ";

            return result;
        }

    }

}
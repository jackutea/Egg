namespace TiedanSouls.Generic {

    /// <summary>
    /// 人物状态标记 (状态标记是一个32位的整数，每一位代表一个状态，可以同时存在多个状态)
    /// </summary>
    [System.Flags]
    public enum RolePositionStatus : int {

        None = 0,                           // 无状态

        OnGround = 1 << 0,                  // 站在地面上
        OnCrossPlatform = 1 << 1,           // 站在平台上
        InWater = 1 << 2,                   // 站在水里

    }

    public static class RolePositionStatusExtension {

        public static bool Contains(this RolePositionStatus stateType, RolePositionStatus stateTypeToCheck) {
            return (stateType & stateTypeToCheck) != 0;
        }

        public static RolePositionStatus AddStatus(this RolePositionStatus stateType, RolePositionStatus stateTypeToAdd) {
            return stateType | stateTypeToAdd;
        }

        public static RolePositionStatus RemoveStatus(this RolePositionStatus stateType, RolePositionStatus stateTypeToRemove) {
            return stateType & ~stateTypeToRemove;
        }

        public static string GetString(this RolePositionStatus stateFlag) {
            if (stateFlag == 0) return "位置状态列表: 无状态";

            string result = "位置状态列表: ";
            if (stateFlag.Contains(RolePositionStatus.OnGround)) result += "站在地面上 ";
            if (stateFlag.Contains(RolePositionStatus.OnCrossPlatform)) result += "站在平台上 ";
            if (stateFlag.Contains(RolePositionStatus.InWater)) result += "站在水面上 ";

            return result;
        }

    }

}
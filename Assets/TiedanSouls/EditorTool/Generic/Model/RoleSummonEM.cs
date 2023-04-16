using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct RoleSummonEM {

        [Header("类型ID")] public int typeID;
        [Header("控制类型")] public ControlType controlType;

    }

}
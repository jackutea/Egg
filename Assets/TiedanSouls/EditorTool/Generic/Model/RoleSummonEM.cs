using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct RoleSummonEM {

        [Header("触发帧")] public int triggerFrame;
        [Header("类型ID")] public int typeID;
        [Header("控制类型")] public ControlType controlType;
        [Header("位置偏移")] public Vector3 localPos;
        [Header("欧拉角偏移")] public Vector3 localEulerAngles;

    }

}
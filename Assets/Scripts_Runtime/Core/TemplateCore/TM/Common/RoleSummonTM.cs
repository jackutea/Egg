using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public struct RoleSummonTM {

        [Header("触发帧")] public int triggerFrame;
        [Header("类型ID")] public int typeID;
        [Header("控制类型")] public ControlType controlType;
        [Header("位置偏移")] public Vector3Int localPosExpanded;
        [Header("欧拉角偏移")] public Vector3Int localEulerAnglesExpanded;

    }

}
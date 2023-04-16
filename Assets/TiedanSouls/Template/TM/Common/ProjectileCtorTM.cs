using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct ProjectileCtorTM {

        [Header("触发帧")] public int triggerFrame;
        [Header("弹幕类型ID")] public int typeID;
        [Header("位置偏移")] public Vector3Int localPosExpanded;
        [Header("欧拉角偏移")] public Vector3Int localEulerAnglesExpanded;

    }

}
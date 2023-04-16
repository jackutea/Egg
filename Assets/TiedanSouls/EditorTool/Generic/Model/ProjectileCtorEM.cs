using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    // TODO: Barrage
    [Serializable]
    public struct ProjectileCtorEM {

        [Header("触发帧")] public int triggerFrame;
        [Header("弹幕ID")] public int typeID;
        [Header("位置偏移")] public Vector3 localPos;
        [Header("欧拉角偏移")] public Vector3 localEulerAngles;

    }

}
using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct ProjectileBulletEM {

        [Header("子弹类型ID")] public int bulletTypeID;
        [Header("触发帧")] public int startFrame;
        [Header("本地坐标")] public Vector3 localPos;
        [Header("本地角度")] public Vector3Int localEulerAngles;

    }

}
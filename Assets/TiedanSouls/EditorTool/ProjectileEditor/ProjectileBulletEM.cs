using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct ProjectileBulletEM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;
        [Header("本地坐标")] public Vector3 localPos;
        [Header("额外穿透次数")] public int extraPenetrateCount;
        [Header("本地角度")] public Vector3Int localEulerAngles;
        [Header("子弹类型ID")] public int bulletTypeID;

    }

}
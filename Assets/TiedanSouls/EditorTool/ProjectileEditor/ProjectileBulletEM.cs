using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct ProjectileBulletEM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;
        [Header("额外打击次数")] public int extraHitTimes;
        [Header("本地坐标(cm)")] public Vector3Int localPos;
        [Header("本地角度")] public Vector3Int localEulerAngles;
        [Header("子弹类型ID")] public int bulletTypeID;

    }

}
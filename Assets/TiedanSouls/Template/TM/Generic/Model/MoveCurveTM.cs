using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct MoveCurveTM {

        [Header("编辑时: 位移(cm)")] public int moveDistance_cm;
        [Header("编辑时: 位移时间(帧)")] public int moveTotalFrame;
        [Header("编辑时: 位移曲线KeyFrameTM")] public KeyframeTM[] disCurve_KeyframeTMArray;

        [Header("移动速度(cm/s)")] public int[] moveSpeedArray;
        [Header("移动方向")] public Vector3Int[] moveDirArray;

    }

}
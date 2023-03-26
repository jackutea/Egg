using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 位移曲线模型
    /// </summary>
    [Serializable]
    public struct MoveCurveEM {

        [Header("位移(m)")] public float moveDistance;
        [Header("位移时间(帧)")] public int moveTotalFrame;
        [Header("位移曲线")] public AnimationCurve disCurve;

    }

}
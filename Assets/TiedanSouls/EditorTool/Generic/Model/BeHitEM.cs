using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 受击模型
    /// </summary>
    [Serializable]
    public struct BeHitEM {

        [Header("受击帧数")] public int beHitTotalFrame;

        // 击退
        [Header("击退距离(m)")] public float knockBackDistance;
        [Header("击退帧数")] public int knockBackTotalFrame;
        [Header("击退位移曲线")] public AnimationCurve knockBackDisCurve;

        [Header("击飞高度(m)")] public float knockUpDis;
        [Header("击飞帧数")] public int knockUpTotalFrame;
        [Header("击飞位移曲线")] public AnimationCurve knockUpDisCurve;

    }

}
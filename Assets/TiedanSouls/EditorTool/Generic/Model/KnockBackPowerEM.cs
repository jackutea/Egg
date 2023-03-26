using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 物理力度模型
    /// </summary>
    [Serializable]
    public struct KnockBackEM {

        // 击退
        [Header("击退距离(m)")] public float knockBackDistance;
        [Header("击退帧数")] public int knockBackCostFrame;
        [Header("击退位移曲线")] public AnimationCurve knockBackDisCurve;

    }

}
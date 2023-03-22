using System;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    /// <summary>
    /// 物理力度模型
    /// </summary>
    [Serializable]
    public struct PhysicsPowerEM {

        // 击退
        [Header("击退距离")] public int knockBackDistance_cm;
        [Header("击退帧数")] public int knockBackCostFrame;
        [Header("击退位移曲线")] public AnimationCurve knockBackDisCurve;

        // 击飞
        [Header("击飞高度")] public int knockUpHeight_cm;
        [Header("击飞帧数")] public int knockUpCostFrame;
        [Header("击飞位移曲线")] public AnimationCurve knockUpDisCurve;

    }

}
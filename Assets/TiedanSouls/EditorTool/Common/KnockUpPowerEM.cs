using System;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    /// <summary>
    /// 击飞力度模型
    /// </summary>
    [Serializable]
    public struct KnockUpEM {

        // 击飞
        [Header("击飞高度")] public int knockUpHeight_cm;
        [Header("击飞帧数")] public int knockUpCostFrame;
        [Header("击飞位移曲线")] public AnimationCurve knockUpDisCurve;

    }

}
using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct SkillModifyEM {

        [Header("技能CD 数值计算类型")] public NumCalculationType cdTime_NCT;
        [Header("技能CD 影响值")] public float cdTime_EV;

    }

}
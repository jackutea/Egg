using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public struct SkillModifyTM {

        [Header("CD 数值计算类型")] public NumCalculationType cdTime_NCT;
        [Header("CD 影响值")] public int cdTime_EV_Expanded;

    }

}
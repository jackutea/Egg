using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// ComparisonType 为 None 时，不进行对比,即不在条件中
    /// </summary>
    [Serializable]
    public struct SkillSelectorEM {

        [Header("技能类型")] public SkillType skillTypeFlag;

    }

}
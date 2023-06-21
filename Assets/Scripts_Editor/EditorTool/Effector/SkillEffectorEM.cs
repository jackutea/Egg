using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct SkillEffectorEM {

        [Header("技能选择")] public SkillSelectorEM skillSelectorEM;
        [Header("技能修改")] public SkillModifyEM skillModifyEM;

    }

}
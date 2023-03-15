using System;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    [Serializable]
    public struct SkillCancelEM {

        [Header("技能类型ID")] public int skillTypeID;
        [Header("是否为组合技")] public bool isCombo;

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

    }

}
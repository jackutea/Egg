using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct SkillCancelEM {

        [Header("技能类型ID")] public int skillTypeID;

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

    }

}
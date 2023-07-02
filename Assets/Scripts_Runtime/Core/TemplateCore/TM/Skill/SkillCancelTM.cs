using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct SkillCancelTM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;
        public SkillCancelType cancelType;
        [Header("技能类型ID")] public int skillTypeID;
    }

}
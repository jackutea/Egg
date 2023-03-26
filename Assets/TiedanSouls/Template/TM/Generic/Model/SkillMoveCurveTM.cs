using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct SkillMoveCurveTM {

        [Header("开始帧")] public int startFrame;
        [Header("是否面向")] public bool isFaceTo;
        [Header("模型: 移动曲线")] public MoveCurveTM moveCurveTM;

    }

}
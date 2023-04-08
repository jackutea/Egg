using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 技能位移曲线模型
    /// </summary>
    [Serializable]
    public struct SkillMoveCurveEM {

        [Header("开始帧")] public int startFrame;
        [Header("是否面向")] public bool isFaceTo;
        [Header("是否等待位移结束")] public bool needWaitForMoveEnd;
        [Header("移动曲线")] public MoveCurveEM moveCurveEM;

    }

}
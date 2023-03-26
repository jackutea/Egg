using System;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    [Serializable]
    public struct SkillMoveCurveModel {

        [Header("开始帧")] public int startFrame;
        [Header("是否面向")] public bool isFaceTo;
        [Header("模型: 移动曲线")] public MoveCurveModel moveCurveModel;

    }

}
using System;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    [Serializable]
    public struct SkillMoveCurveModel {

        [Header("触发帧")] public int triggerFrame;
        [Header("是否面向")] public bool isFaceTo;
        [Header("是否等待位移结束")] public bool needWaitForMoveEnd;
        [Header("移动曲线")] public MoveCurveModel moveCurveModel;

    }

}
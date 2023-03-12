using System;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillorEditor {

    [Serializable]
    public class CollisionTriggerEM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("触发间隔")] public int triggerIntervalFrame;
        [Header("触发时间")] public int triggerMaintainFrame;

        [Header("碰撞盒")] public GameObject[] colliderGOArray;

    }

}
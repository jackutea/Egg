using System;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    [Serializable]
    public class CollisionTriggerEM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("触发间隔")] public int triggerIntervalFrame;
        [Header("触发时间")] public int triggerMaintainFrame;

        [Header("打击力度")] public HitPowerEM hitPowerEM;

        [Header("碰撞器(组) 注: 相对路径不能重复!")] public GameObject[] colliderGOArray;

    }

}
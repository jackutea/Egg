using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    [Serializable]
    public struct CollisionTriggerEM {

        [Header("启用/关闭")] public bool isEnabled;

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("延迟帧数")] public int delayFrame;
        [Header("触发间隔")] public int intervalFrame;
        [Header("触发时间")] public int maintainFrame;

        [Header("打击目标类型")] public TargetType hitTargetType;
        [Header("碰撞器(组) 注: 相对路径不能重复!")] public GameObject[] colliderGOArray;
        [Header("打击力度")] public HitPowerEM hitPowerEM;

    }

}
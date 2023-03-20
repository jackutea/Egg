using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct CollisionTriggerTM {

        [Header("启用/关闭")] public bool isEnabled;
        
        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("触发延迟")] public int delayFrame;
        [Header("触发间隔")] public int intervalFrame;
        [Header("触发时间")] public int maintainFrame;

        [Header("碰撞盒(组)")] public ColliderTM[] colliderTMArray;
        [Header("打击力度")] public HitPowerTM hitPowerTM;
        [Header("打击目标类型")] public TargetType hitTargetType;

        [Header("(仅用于编辑器) 碰撞盒相对路径(组)")] public string[] colliderRelativePathArray;

    }

}
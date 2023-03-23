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

        [Header("作用目标")] public TargetGroupType targetGroupType;
        [Header("模型: 伤害")] public DamageTM damageTM;
        [Header("模型: 击退力度")] public KnockBackPowerTM knockBackPowerTM;
        [Header("模型: 击飞力度")] public KnockUpPowerTM knockUpPowerTM;
        [Header("模型: 效果器")] public EffectorTM hitEffectorTM;
        [Header("模型: 状态影响")] public StateEffectTM stateEffectTM;

        [Header("碰撞盒(组)")] public ColliderTM[] colliderTMArray;

        [Header("(仅用于编辑器) 碰撞盒相对路径(组)")] public string[] colliderRelativePathArray;

    }

}
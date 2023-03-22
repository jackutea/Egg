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

        [Header("碰撞盒(组) 注: 相对路径不能重复!")] public GameObject[] colliderGOArray;

        [Header("作用目标")] public TargetGroupType targetGroupType;
        [Header("模型: 伤害")] public DamageEM damageEM;
        [Header("模型: 物理力度")] public KnockBackPowerEM knockBackPowerEM;
        [Header("模型: 击飞力度")] public KnockUpPowerEM knockUpPowerEM;
        [Header("模型: 击中效果器")] public EffectorEM hitEffectorEM;
        [Header("模型: 状态影响")] public StateEffectEM stateEffectEM;

    }

}
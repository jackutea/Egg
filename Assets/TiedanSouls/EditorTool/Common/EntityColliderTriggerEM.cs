using System;
using TiedanSouls.Generic;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 碰撞触发器模型
    /// </summary>
    [Serializable]
    public struct EntityColliderTriggerEM {

        [Header("启用/关闭")] public bool isEnabled;

        [Header("帧区间")] public Vector2Int frameRange;

        [Header("触发模式")] public TriggerMode triggerMode;
        [Header("Custom:自定义触发")] public TriggerCustomEM triggerCustomEM;
        [Header("FixedInterval:固定间隔触发")] public TriggerFixedIntervalEM triggerFixedIntervalEM;

        [Header("作用实体类型")] public EntityType targetEntityType;
        [Header("作用目标群体")] public AllyType hitAllyType;

        [Header("作用目标效果器组")] public int[] targetRoleEffectorTypeIDArray;
        [Header("作用自身效果器组")] public int[] selfRoleEffectorTypeIDArray;

        [Header("伤害")] public DamageEM damageEM;
        [Header("打击")] public BeHitEM beHitEM;
        [Header("控制效果(组)")] public RoleCtrlEffectEM[] roleCtrlEffectEMArray;

        [Header("碰撞盒(组) 注: 相对路径不能重复!")] public GameObject[] colliderGOArray;

    }

}
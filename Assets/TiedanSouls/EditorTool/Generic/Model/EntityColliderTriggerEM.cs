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
        [Header("模型: 固定间隔触发")] public TriggerFixedIntervalEM triggerFixedIntervalEM;
        [Header("模型: 自定义触发")] public TriggerCustomEM triggerCustomEM;

        [Header("作用实体类型")] public EntityType targetEntityType;
        [Header("作用目标群体")] public TargetGroupType hitTargetGroupType;
        
        [Header("模型: 伤害")] public DamageEM damageEM;
        [Header("模型: 击退")] public KnockBackEM knockBackEM;
        [Header("模型: 击飞")] public KnockUpEM knockUpEM;
        [Header("模型: 击中效果器")] public int hitEffectorTypeID;

        [Header("碰撞盒(组) 注: 相对路径不能重复!")] public GameObject[] colliderGOArray;

    }

}
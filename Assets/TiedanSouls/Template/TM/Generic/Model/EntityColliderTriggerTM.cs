using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    /// <summary>
    /// 碰撞触发器模型
    /// </summary>
    [Serializable]
    public struct EntityColliderTriggerTM {

        [Header("启用/关闭")] public bool isEnabled;

        [Header("帧区间")] public Vector2Int frameRange;

        [Header("触发模式")] public TriggerMode triggerMode;
        [Header("固定间隔")] public TriggerFixedIntervalTM triggerFixedIntervalTM;
        [Header("自定义")] public TriggerCustomTM triggerCustomTM;

        [Header("作用实体类型")] public EntityType targetEntityType;
        [Header("作用目标")] public TargetGroupType hitTargetGroupType;

        [Header("伤害")] public DamageTM damageTM;
        [Header("受击")] public BeHitTM beHitTM;
        [Header("控制效果(组)")] public RoleCtrlEffectTM[] roleCtrlEffectTMArray;
        [Header("击中效果器")] public int hitEffectorTypeID;

        [Header("碰撞盒(组)")] public ColliderTM[] colliderTMArray;

        [Header("(仅用于编辑器) 碰撞盒相对路径(组)")] public string[] colliderRelativePathArray;

    }

}
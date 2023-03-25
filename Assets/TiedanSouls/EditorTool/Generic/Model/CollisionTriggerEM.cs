using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 碰撞触发器模型
    /// </summary>
    [Serializable]
    public struct CollisionTriggerEM {

        [Header("启用/关闭")] public bool isEnabled;

        [Header("总时间(帧)")] public int totalFrame;

        // TODO: 添加自定义设置触发帧
        [Header("延时触发(帧)")] public int delayFrame;
        [Header("触发间隔(帧)")] public int intervalFrame;
        [Header("单次触发时间(帧)")] public int maintainFrame;

        [Header("作用目标")] public TargetGroupType targetGroupType;
        [Header("模型: 伤害")] public DamageEM damageEM;
        [Header("模型: 物理力度")] public KnockBackEM knockBackPowerEM;
        [Header("模型: 击飞力度")] public KnockUpEM knockUpPowerEM;
        [Header("模型: 击中效果器")] public int hitEffectorTypeID;
        [Header("模型: 状态影响")] public StateEffectEM stateEffectEM;

        [Header("碰撞盒(组) 注: 相对路径不能重复!")] public GameObject[] colliderGOArray;

    }

}
using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 属性影响模型
    /// </summary>
    [Serializable]
    public struct AttributeEffectEM {

        [Header("生命值 数值计算类型")] public NumCalculationType hpNCT;
        [Header("生命值影响值")] public float hpEV;

        [Header("最大生命值 数值计算类型")] public NumCalculationType maxHpNCT;
        [Header("最大生命值 影响值")] public float maxHpEV;

        [Header("攻击力 数值计算类型")] public NumCalculationType atkPowerNCT;
        [Header("攻击力影响值")] public float atkPowerEV;

        [Header("攻击速度 数值计算类型")] public NumCalculationType atkSpeedNCT;
        [Header("攻击速度 影响值")] public float atkSpeedEV;

        [Header("移动速度 数值计算类型")] public NumCalculationType moveSpeedNCT;
        [Header("移动速度 影响值")] public float moveSpeedEV;

    }

}
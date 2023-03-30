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
        [Header("生命值 影响值")] public int hpEV;
        [Header("结束时撤销")] public bool needRevokeHPEV;
        [Header("影响次数")] public int hpEffectTimes;

        [Header("最大生命值 数值计算类型")] public NumCalculationType hpMaxNCT;
        [Header("最大生命值 影响值")] public int hpMaxEV;
        [Header("结束时撤销")] public bool needRevokeHPMaxEV;
        [Header("影响次数")] public int hpMaxEffectTimes;

        [Header("攻击力 数值计算类型")] public NumCalculationType atkPowerNCT;
        [Header("攻击力 影响值")] public int atkPowerEV;
        [Header("是否需要撤销 攻击力影响值")] public bool needRevokeAtkPowerEV;
        [Header("影响次数")] public int atkPowerEffectTimes;

        [Header("攻击速度 数值计算类型")] public NumCalculationType atkSpeedNCT;
        [Header("攻击速度 影响值")] public int atkSpeedEV;
        [Header("结束时撤销")] public bool needRevokeAtkSpeedEV;
        [Header("影响次数")] public int atkSpeedEffectTimes;

        [Header("移动速度 数值计算类型")] public NumCalculationType moveSpeedNCT;
        [Header("移动速度 影响值")] public int moveSpeedEV;
        [Header("结束时撤销")] public bool needRevokeMoveSpeedEV;
        [Header("影响次数")] public int moveSpeedEffectTimes;

    }

}
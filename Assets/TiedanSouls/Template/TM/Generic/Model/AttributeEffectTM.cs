using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    /// <summary>
    /// 属性影响模型
    /// </summary>
    [Serializable]
    public struct AttributeEffectTM {

        [Header("生命值 数值计算类型")] public NumCalculationType hpNCT;
        [Header("生命值 影响值")] public int hpEV;
        [Header("结束时撤销")] public bool needRevokeHPEV;
        [Header("影响次数")] public int hpEffectTimes;

        [Header("最大生命值 数值计算类型")] public NumCalculationType hpMaxNCT;
        [Header("最大生命值 影响值")] public int hpMAXEV;
        [Header("结束时撤销")] public bool needRevokeHPMaxEV;
        [Header("影响次数")] public int hpMaxEffectTimes;

        [Header("AtkPower 数值计算类型")] public NumCalculationType atkPowerNCT;
        [Header("AtkPower 影响值")] public int atkPowerEV;
        [Header("结束时撤销")] public bool needRevokeAtkPowerEV;
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
using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 武器属性影响模型
    /// </summary>
    [Serializable]
    public struct WeaponAttributeEffectEM {

        [Header("物理伤害加成 数值计算类型")] public NumCalculationType physicsDamageIncreaseNCT;
        [Header("物理伤害加成 影响值")] public int physicsDamageIncreaseEV;
        [Header("影响次数")] public int physicsDamageIncreaseEffectTimes;
        [Header("结束时撤销")] public bool needRevokePhysicsDamageIncreaseEV;

        [Header("魔法伤害加成 数值计算类型")] public NumCalculationType magicDamageIncreaseNCT;
        [Header("魔法伤害加成 影响值")] public int magicDamageIncreaseEV;
        [Header("影响次数")] public int magicDamageIncreaseEffectTimes;
        [Header("结束时撤销")] public bool needRevokeMagicDamageIncreaseEV;

        [Header("攻击速度 数值计算类型")] public NumCalculationType atkSpeedNCT;
        [Header("攻击速度 影响值")] public int atkSpeedEV;
        [Header("影响次数")] public int atkSpeedEffectTimes;
        [Header("结束时撤销")] public bool needRevokeAtkSpeedEV;


    }

}
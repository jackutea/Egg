using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    /// <summary>
    /// 武器属性影响模型
    /// </summary>
    [Serializable]
    public struct WeaponAttributeEffectTM {

        [Header("物理伤害加成 影响值")] public int physicsDamageIncreaseEV;
        [Header("影响次数")] public int physicsDamageIncreaseEffectTimes;
        [Header("结束时撤销")] public bool needRevokePhysicsDamageIncreaseEV;

        [Header("魔法伤害加成 影响值")] public int magicDamageIncreaseEV;
        [Header("影响次数")] public int magicDamageIncreaseEffectTimes;
        [Header("结束时撤销")] public bool needRevokeMagicDamageIncreaseEV;

    }

}
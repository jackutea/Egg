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
        [Header("结束时撤销")] public bool needRevokeHP;
        [Header("影响次数")] public int hpEffectTimes;

        [Header("最大生命值 数值计算类型")] public NumCalculationType hpMaxNCT;
        [Header("最大生命值 影响值")] public int hpMAXEV;
        [Header("结束时撤销")] public bool needRevokeHPMax;
        [Header("影响次数")] public int hpMaxEffectTimes;

        [Header("移动速度 数值计算类型")] public NumCalculationType moveSpeedNCT;
        [Header("移动速度 影响值")] public int moveSpeedEV;
        [Header("结束时撤销")] public bool needRevokeMoveSpeed;
        [Header("影响次数")] public int moveSpeedEffectTimes;

        [Header("物理伤害加成 影响值")] public int physicsDamageBonusEV;
        [Header("影响次数")] public int physicsDamageBonusEffectTimes;
        [Header("结束时撤销")] public bool needRevokePhysicsDamageBonus;

        [Header("魔法伤害加成 影响值")] public int magicDamageBonusEV;
        [Header("影响次数")] public int magicDamageBonusEffectTimes;
        [Header("结束时撤销")] public bool needRevokeMagicDamageBonus;

        [Header("物理防御加成 影响值")] public int physicsDefenseBonusEV;
        [Header("影响次数")] public int physicsDefenseBonusEffectTimes;
        [Header("结束时撤销")] public bool needRevokePhysicsDefenseBonus;

        [Header("魔法防御加成 影响值")] public int magicDefenseBonusEV;
        [Header("影响次数")] public int magicDefenseBonusEffectTimes;
        [Header("结束时撤销")] public bool needRevokeMagicDefenseBonus;

    }

}
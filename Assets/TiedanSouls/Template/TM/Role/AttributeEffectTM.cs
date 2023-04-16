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
        [Header("生命值 影响值")] public int hpEV_Expanded;
        [Header("结束时撤销")] public bool NeedRevokeHP;
        [Header("影响次数")] public int hpEffectTimes;

        [Header("最大生命值 数值计算类型")] public NumCalculationType hpMaxNCT;
        [Header("最大生命值 影响值")] public int hpMaxEV_Expanded;
        [Header("结束时撤销")] public bool NeedRevokeHPMax;
        [Header("影响次数")] public int hpMaxEffectTimes;

        [Header("移动速度 数值计算类型")] public NumCalculationType moveSpeedNCT;
        [Header("移动速度 影响值")] public int moveSpeedEV_Expanded;
        [Header("结束时撤销")] public bool NeedRevokeMoveSpeed;
        [Header("影响次数")] public int moveSpeedEffectTimes;

        [Header("普攻速度加成")] public int normalSkillSpeedBonusEV_Expanded;
        [Header("影响次数")] public int normalSkillSpeedBonusEffectTimes;
        [Header("结束时撤销")] public bool normalSkillSpeedBonusNeedRevoke;

        [Header("物理伤害加成 影响值")] public int physicalDamageBonusEV_Expanded;
        [Header("影响次数")] public int physicalDamageBonusEffectTimes;
        [Header("结束时撤销")] public bool physicalDamageBonusNeedRevoke;

        [Header("魔法伤害加成 影响值")] public int magicalDamageBonusEV_Expanded;
        [Header("影响次数")] public int magicalDamageBonusEffectTimes;
        [Header("结束时撤销")] public bool magicalDamageBonusNeedRevoke;

        [Header("物理防御加成 影响值")] public int physicalDefenseBonusEV_Expanded;
        [Header("影响次数")] public int physicalDefenseBonusEffectTimes;
        [Header("结束时撤销")] public bool needRevokePhysicalDefenseBonus;

        [Header("魔法防御加成 影响值")] public int magicalDefenseBonusEV_Expanded;
        [Header("影响次数")] public int magicalDefenseBonusEffectTimes;
        [Header("结束时撤销")] public bool magicalDefenseBonusNeedRevoke;

    }

}
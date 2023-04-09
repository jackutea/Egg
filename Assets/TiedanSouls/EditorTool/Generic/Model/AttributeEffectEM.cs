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
        [Header("生命值 影响值")] public float hpEV;
        [Header("影响次数")] public int hpEffectTimes;
        [Header("结束时撤销")] public bool hpNeedRevoke;
        [Header(" ================================================================================== ")]
        [Header("最大生命值 数值计算类型")] public NumCalculationType hpMaxNCT;
        [Header("最大生命值 影响值")] public float hpMaxEV;
        [Header("影响次数")] public int hpMaxEffectTimes;
        [Header("结束时撤销")] public bool hpMaxNeedRevoke;
        [Header(" ================================================================================== ")]
        [Header("移动速度 数值计算类型")] public NumCalculationType moveSpeedNCT;
        [Header("移动速度 影响值")] public float moveSpeedEV;
        [Header("影响次数")] public int moveSpeedEffectTimes;
        [Header("结束时撤销")] public bool moveSpeedRevoke;
        [Header(" ================================================================================== ")]
        [Header("普攻速度加成 影响值(0-1)")] public float normalSkillSpeedBonusEV;
        [Header("影响次数")] public int normalSkillSpeedBonusEffectTimes;
        [Header("结束时撤销")] public bool normalSkillSpeedBonusNeedRevoke;
        [Header(" ================================================================================== ")]
        [Header("物理伤害加成 影响值(0-1)")] public float physicalDamageBonusEV;
        [Header("影响次数")] public int physicalDamageBonusEffectTimes;
        [Header("结束时撤销")] public bool physicalDamageBonusNeedRevoke;
        [Header(" ================================================================================== ")]
        [Header("魔法伤害加成 影响值(0-1)")] public float magicalDamageBonusEV;
        [Header("影响次数")] public int magicalDamageBonusEffectTimes;
        [Header("结束时撤销")] public bool magicalDamageBonusNeedRevoke;
        [Header(" ================================================================================== ")]
        [Header("物理防御加成 影响值(0-1)")] public float physicalDefenseBonusEV;
        [Header("影响次数")] public int physicalDefenseBonusEffectTimes;
        [Header("结束时撤销")] public bool physicalDefenseBonusNeedRevoke;
        [Header(" ================================================================================== ")]
        [Header("魔法防御加成 影响值(0-1)")] public float magicalDefenseBonusEV;
        [Header("影响次数")] public int magicalDefenseBonusEffectTimes;
        [Header("结束时撤销")] public bool magicalDefenseBonusNeedRevoke;

    }

}
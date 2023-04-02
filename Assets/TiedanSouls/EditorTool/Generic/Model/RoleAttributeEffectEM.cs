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
        [Header("结束时撤销")] public bool needRevoke_HPEV;
        [Header("影响次数")] public int hpEffectTimes;
        [Header(" ================================================================================== ")]
        [Header("最大生命值 数值计算类型")] public NumCalculationType hpMaxNCT;
        [Header("最大生命值 影响值")] public int hpMaxEV;
        [Header("结束时撤销")] public bool needRevoke_HPMaxEV;
        [Header("影响次数")] public int hpMaxEffectTimes;
        [Header(" ================================================================================== ")]
        [Header("移动速度 数值计算类型")] public NumCalculationType moveSpeedNCT;
        [Header("移动速度 影响值")] public int moveSpeedEV;
        [Header("结束时撤销")] public bool needRevoke_MoveSpeedEV;
        [Header("影响次数")] public int moveSpeedEffectTimes;
        [Header(" ================================================================================== ")]
        [Header("物理伤害加成 影响值(百分比)")] public int physicsDamageBonusEV;
        [Header("影响次数")] public int physicsDamageBonusEffectTimes;
        [Header("结束时撤销")] public bool needRevokePhysicsDamageBonus;
        [Header(" ================================================================================== ")]
        [Header("魔法伤害加成 影响值(百分比)")] public int magicDamageBonusEV;
        [Header("影响次数")] public int magicDamageBonusEffectTimes;
        [Header("结束时撤销")] public bool needRevokeMagicDamageBonus;
        [Header(" ================================================================================== ")]
        [Header("物理防御加成 影响值(百分比)")] public int physicsDefenseBonusEV;
        [Header("影响次数")] public int physicsDefenseBonusEffectTimes;
        [Header("结束时撤销")] public bool needRevokePhysicsDefenseBonus;
        [Header(" ================================================================================== ")]
        [Header("魔法防御加成 影响值(百分比)")] public int magicDefenseBonusEV;
        [Header("影响次数")] public int magicDefenseBonusEffectTimes;
        [Header("结束时撤销")] public bool needRevokeMagicDefenseBonus;

    }

}
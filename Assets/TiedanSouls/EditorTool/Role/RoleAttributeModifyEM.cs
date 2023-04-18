using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 属性影响模型
    /// </summary>
    [Serializable]
    public struct RoleAttributeModifyEM {

        [Header("生命值 数值计算类型")] public NumCalculationType hpNCT;
        [Header("生命值 影响值")] public float hpEV;
        [Header(" ================================================================================== ")]
        [Header("最大生命值 数值计算类型")] public NumCalculationType hpMaxNCT;
        [Header("最大生命值 影响值")] public float hpMaxEV;
        [Header(" ================================================================================== ")]
        [Header("移动速度 数值计算类型")] public NumCalculationType moveSpeedNCT;
        [Header("移动速度 影响值")] public float moveSpeedEV;
        [Header(" ================================================================================== ")]
        [Header("普攻速度加成 影响值(0-1)")] public float normalSkillSpeedBonusEV;
        [Header(" ================================================================================== ")]
        [Header("物理伤害加成 影响值(0-1)")] public float physicalDamageBonusEV;
        [Header(" ================================================================================== ")]
        [Header("魔法伤害加成 影响值(0-1)")] public float magicalDamageBonusEV;
        [Header(" ================================================================================== ")]
        [Header("物理防御加成 影响值(0-1)")] public float physicalDefenseBonusEV;
        [Header(" ================================================================================== ")]
        [Header("魔法防御加成 影响值(0-1)")] public float magicalDefenseBonusEV;

    }

}
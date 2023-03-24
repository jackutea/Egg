using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct DamageEM {

        [Header("伤害类型")] public DamageType damageType;
        [Header("基础伤害")] public int damageBase;
        [Header("基础伤害 - 曲线")] public AnimationCurve damageCurve;

    }

}
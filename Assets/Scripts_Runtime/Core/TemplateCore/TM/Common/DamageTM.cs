using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct DamageTM {

        [Header("伤害类型")] public DamageType damageType;
        [Header("伤害值(组)")] public int[] damageArray;
        
        [Header("仅用于编辑时 基础伤害")] public int damageBase;
        [Header("仅用于编辑时 伤害 KeyFrameTM")] public KeyframeTM[] damageCurve_KeyframeTMArray;

    }

}
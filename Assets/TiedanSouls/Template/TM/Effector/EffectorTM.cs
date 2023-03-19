using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct EffectorTM {

        [Header("实体生成模型(组)")] public EntitySummonTM[] entitySummonTMArray;
        [Header("实体销毁模型(组)")] public EntityDestroyTM[] entityDestroyTMArray;

    }

}
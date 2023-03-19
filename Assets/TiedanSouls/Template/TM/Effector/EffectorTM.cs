using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct EffectorTM {

        [Header("类型ID")] public int typeID;
        [Header("效果器名称")] public string effectorName;
        [Header("实体生成模型(组)")] public EntitySummonTM[] entitySummonTMArray;
        [Header("实体销毁模型(组)")] public EntityDestroyTM[] entityDestroyTMArray;

    }

}
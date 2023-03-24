using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct EffectorEM {

        [Header("类型ID")] public int typeID;
        [Header("效果器名称")] public string effectorName;
        [Header("实体生成模型(组)")] public EntitySummonEM[] entitySummonEMArray;
        [Header("实体销毁模型(组)")] public EntityDestroyEM[] entityDestroyEMArray;

    }

}
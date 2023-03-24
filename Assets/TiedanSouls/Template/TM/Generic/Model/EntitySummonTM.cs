using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct EntitySummonTM {

        [Header("召唤实体类型")] public EntityType entityType;
        [Header("实体类型ID")] public int typeID;
        [Header("控制类型")] public ControlType controlType;

    }

}
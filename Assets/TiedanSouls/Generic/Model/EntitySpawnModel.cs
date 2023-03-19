using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    // Use for Entity Spawn.
    [Serializable]
    public struct EntitySpawnModel {

        [Header("是否为Boss")]
        public bool isBoss;

        [Header("实体类型")]
        public EntityType entityType;

        [Header("具体实体类型ID")]
        public int typeID;

        [Header("控制类型")]
        public ControlType controlType;

        [Header("阵营")]
        public AllyType allyType;

    }

}
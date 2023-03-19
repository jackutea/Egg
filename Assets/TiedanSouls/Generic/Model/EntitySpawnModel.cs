using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    // Use for Entity Spawn.
    [Serializable]
    public struct EntitySpawnModel {

        public EntityType entityType;
        public int typeID;
        public ControlType controlType;
        public AllyType allyType;

    }

}
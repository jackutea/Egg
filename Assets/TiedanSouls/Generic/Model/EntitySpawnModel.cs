using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    [Serializable]
    public struct EntitySpawnModel {

        public EntityType entityType;
        public int typeID;
        public ControlType controlType;
        public AllyType allyType;
        public Vector3 spawnPos;
        public bool isBoss;

    }

}
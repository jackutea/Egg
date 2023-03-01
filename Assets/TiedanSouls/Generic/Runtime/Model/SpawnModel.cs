using System;
using UnityEngine;

namespace TiedanSouls {

    // Use for Entity Spawn.
    [Serializable]
    public struct SpawnModel {

        public EntityType entityType;
        public RoleControlType roleControlType;
        public AllyType allyType;
        public int typeID;
        public Vector2 pos;

        public int spawnFrame;  // 从进入关卡到出生的帧数

    }

}
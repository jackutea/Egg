using System;
using UnityEngine;

namespace TiedanSouls {

    // Use for Entity Spawn.
    [Serializable]
    public struct SpawnModel {

        public EntityType entityType;
        public RoleControlType roleControlType;
        public Vector2 pos;

    }

}
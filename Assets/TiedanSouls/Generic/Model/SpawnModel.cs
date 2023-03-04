using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    // Use for Entity Spawn.
    [Serializable]
    public struct SpawnModel {

        public EntityType entityType;
        public int typeID;

        public ControlType controlType;
        public AllyType allyType;

        public Vector2 pos;

        public int spawnFrame;  // 从进入关卡到出生的帧数

    }

}
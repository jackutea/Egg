using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class FieldTM {

        public int typeID;
        public FieldType fieldType;

        public ushort chapter;
        public ushort level;

        // Spawn Info
        public SpawnModel[] spawnModelArray;
        public Vector2[] itemSpawnPosArray;

        public string fieldAssetName;

    }

}
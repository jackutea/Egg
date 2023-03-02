using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class FieldTM {

        public string fieldAssetName;

        public int typeID;
        public FieldType fieldType;

        public ushort chapter;
        public ushort level;

        // Spawn
        public SpawnModel[] spawnModelArray;
        public Vector2[] itemSpawnPosArray;

        // Field Link
        public FieldDoorModel[] fieldDoorArray;

    }

}
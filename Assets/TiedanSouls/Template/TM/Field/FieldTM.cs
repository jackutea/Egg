using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public class FieldTM {

        public string fieldAssetName;

        public int typeID;
        public FieldType fieldType;

        public ushort chapter;
        public ushort level;

        // Spawn
        public EntitySpawnCtrlModel[] spawnCtrlModelArray;
        public Vector2[] itemSpawnPosArray;

        // Field Link
        public FieldDoorModel[] fieldDoorArray;

    }

}
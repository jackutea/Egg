using System;

namespace TiedanSouls.Template {

    [Serializable]
    public class FieldTM {

        public int typeID;
        public FieldType fieldType;

        public ushort chapter;
        public ushort level;

        public SpawnModel[] spawnModelArray;
        public string fieldAssetName;

    }

}
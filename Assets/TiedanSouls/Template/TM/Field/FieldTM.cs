using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public class FieldTM {

        public string fieldAssetName;

        public int typeID;
        public FieldType fieldType;

        public ushort chapter;                              // 章节
        public ushort level;                                // 关卡

        public EntitySpawnCtrlTM[] entitySpawnCtrlTMArray;  // 实体生成控制模型(组)
        public Vector2[] itemSpawnPosArray;                 // 物品生成位置(组) 

        public FieldDoorModel[] fieldDoorArray;             // 场景门模型(组)

    }

}
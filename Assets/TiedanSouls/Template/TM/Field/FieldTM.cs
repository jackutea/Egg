using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public struct FieldTM {

        public string fieldAssetName;

        public int typeID;
        public FieldType fieldType;

        [Header("章节")] public ushort chapter;
        [Header("关卡")] public ushort level;

        [Header("实体生成控制模型(组)")] public EntitySpawnCtrlTM[] entitySpawnCtrlTMArray;
        [Header("物品生成位置(组)")] public Vector2[] itemSpawnPosArray;
        [Header("场景门模型(组)")] public FieldDoorTM[] fieldDoorTMArray;

    }

}
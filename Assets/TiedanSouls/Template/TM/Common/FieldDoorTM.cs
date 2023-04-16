using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct FieldDoorTM {

        [Header("场景类型ID")] public int fieldTypeID;
        [Header("位置")] public Vector3Int pos_cm;
        [Header("门索引")] public int doorIndex;

    }

}
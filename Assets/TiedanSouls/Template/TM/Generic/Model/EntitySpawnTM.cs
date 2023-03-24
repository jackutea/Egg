using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    /// <summary>
    /// 实体生成模型
    /// </summary>
    [Serializable]
    public struct EntitySpawnTM {

        [Header("实体类型")] public EntityType entityType;

        [Header("具体实体类型ID")] public int typeID;

        [Header("控制类型")] public ControlType controlType;

        [Header("阵营")] public AllyType allyType;

        [Header("是否为Boss")] public bool isBoss;

        [Header("出生位置")] public Vector3 spawnPos;


    }

}
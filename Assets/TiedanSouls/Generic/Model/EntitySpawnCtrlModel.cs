using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    [Serializable]
    public struct EntitySpawnCtrlModel {

        [Header("是否为Boss")] public bool isBoss;
        [Header("出生帧")] public int spawnFrame;
        [Header("出生位置")] public Vector2 pos;
        [Header("关卡断点(杀死此刻前(包括此刻)的所有怪物才能推进关卡)")] public bool isBreakPoint;

        #region [抽象为实体生成模型(EntitySpawnModel)]

        [Header("实体类型")] public EntityType entityType;

        [Header("具体实体类型ID")] public int typeID;

        [Header("控制类型")] public ControlType controlType;

        [Header("阵营")] public AllyType allyType;

        #endregion

    }

}
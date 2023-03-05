using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    // Use for Entity Spawn.
    [Serializable]
    public struct SpawnModel {

        [Header("实体类型")]
        public EntityType entityType;

        [Header("具体实体类型ID")]
        public int typeID;

        [Header("控制类型")]
        public ControlType controlType;

        [Header("阵营")]
        public AllyType allyType;

        [Header("出生位置")]
        public Vector2 pos;

        [Header("出生帧")]
        public int spawnFrame;  // 从进入关卡到出生的帧数

        [Header("关卡断点(杀死此刻前(包括此刻)的所有怪物才能推进关卡)")]
        public bool isBreakPoint;

    }

}
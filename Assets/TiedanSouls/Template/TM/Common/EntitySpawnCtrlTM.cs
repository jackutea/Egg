using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct EntitySpawnCtrlTM {

        [Header("出生帧")] public int spawnFrame;
        [Header("关卡断点(杀死此刻前(包括此刻)的所有怪物才能推进关卡)")] public bool isBreakPoint;

        [Header("实体生成模型")] public EntitySpawnTM entitySpawnTM;

    }

}
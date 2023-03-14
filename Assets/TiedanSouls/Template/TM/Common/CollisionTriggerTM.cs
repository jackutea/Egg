using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class CollisionTriggerTM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("触发间隔")] public int triggerIntervalFrame;
        [Header("触发时间")] public int triggerMaintainFrame;

        [Header("碰撞盒(组)")] public ColliderTM[] colliderTMArray;

        [Header("(仅用于编辑器) 碰撞盒相对路径(组)")] public string[] colliderRelativePathArray;

    }

}
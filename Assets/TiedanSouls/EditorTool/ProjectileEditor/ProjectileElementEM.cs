using System;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    [Serializable]
    public struct ProjectileElementEM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("碰撞器")] public CollisionTriggerEM collisionTriggerEM;

        [Header("打击效果器")] public EffectorEM hitEffectorEM;
        [Header("死亡效果器")] public EffectorEM deadEffectorEM;

        [Header("额外打击次数")] public int extraHitTimes;

        [Header("弹道元素特效")] public GameObject vfxPrefab;

    }

}
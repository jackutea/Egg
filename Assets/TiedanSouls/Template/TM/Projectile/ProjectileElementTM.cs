using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct ProjectileElementTM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("碰撞器")] public CollisionTriggerTM collisionTriggerTM;

        [Header("打击效果器")] public EffectorTM hitEffectorTM;
        [Header("死亡效果器")] public EffectorTM deathEffectorTM;

        [Header("额外打击次数")] public int extraHitTimes;

        [Header("弹道元素特效")] public string vfxPrefabName;
       [Header("仅用于编辑时: 弹道元素特效文件GUID")]public string vfxPrefab_GUID;

    }

}
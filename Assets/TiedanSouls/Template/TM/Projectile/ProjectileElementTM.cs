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
        [Header("仅用于编辑时: 弹道元素特效文件GUID")] public string vfxPrefab_GUID;

        [Header("位移(cm)")] public int moveDistance;
        [Header("位移时间(帧)")] public int moveTotalFrame;
        [Header("速度(组)")] public int[] moveSpeedArray_cm;
        [Header("仅用于编辑时: 位移曲线KeyFrameTM")] public KeyframeTM[] keyframeTMArray;

    }

}
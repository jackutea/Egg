using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct BulletTM {

        [Header("碰撞器")] public CollisionTriggerTM collisionTriggerTM;
        [Header("打击效果器")] public EffectorTM hitEffectorTM;
        [Header("死亡效果器")] public EffectorTM deathEffectorTM;

        [Header("仅用于编辑时: 位移(cm)")] public int moveDistance_cm;
        [Header("仅用于编辑时: 位移时间(帧)")] public int moveTotalFrame;
        [Header("速度组(cm)")] public int[] moveSpeedArray_cm;
        [Header("方向组")] public Vector3Int[] directionArray;
        [Header("仅用于编辑时: 位移曲线KeyFrameTM")] public KeyframeTM[] disCurve_KeyframeTMArray;

        [Header("子弹特效")] public string vfxPrefabName;
        [Header("仅用于编辑时: 子弹特效文件GUID")] public string vfxPrefab_GUID;

    }

}
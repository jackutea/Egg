using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct BulletEM {

        [Header("碰撞器")] public CollisionTriggerEM collisionTriggerEM;

        [Header("打击效果器")] public EffectorEM hitEffectorEM;
        [Header("死亡效果器")] public EffectorEM deathEffectorEM;

        [Header("子弹特效")] public GameObject vfxPrefab;

        [Header("位移(cm)")] public int moveDistance_cm;
        [Header("位移时间(帧)")] public int moveTotalFrame;
        [Header("位移曲线")] public AnimationCurve disCurve;

    }

}
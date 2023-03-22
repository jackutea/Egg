using System;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    [Serializable]
    public struct ProjectileElementEM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("碰撞器")] public CollisionTriggerEM collisionTriggerEM;

        [Header("打击效果器")] public EffectorEM hitEffectorEM;
        [Header("死亡效果器")] public EffectorEM deathEffectorEM;

        [Header("额外打击次数")] public int extraHitTimes;

        [Header("弹道元素特效")] public GameObject vfxPrefab;

        [Header("位移(cm)")] public int moveDistance_cm;
        [Header("位移时间(帧)")] public int moveTotalFrame;
        [Header("位移曲线")] public AnimationCurve moveCurve;

        [Header("相对偏移: 位置(cm)")] public Vector3Int relativeOffset_pos;
        [Header("相对偏移: 旋转(度)")] public Vector3Int relativeOffset_euler; 

    }

}
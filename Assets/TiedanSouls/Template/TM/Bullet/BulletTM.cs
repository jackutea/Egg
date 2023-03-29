using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public struct BulletTM {

        [Header("类型ID")] public int typeID;
        [Header("子弹名称")] public string bulletName;

        [Header("轨迹类型")] public TrajectoryType trajectoryType;

        [Header("模型: 位移曲线")] public MoveCurveTM moveCurveTM;
        [Header("实体追踪模型")] public EntityTrackTM entityTrackTM;
        [Header("碰撞器")] public CollisionTriggerTM collisionTriggerTM;

        [Header("死亡效果器")] public int deathEffectorTypeID;

        [Header("子弹特效")] public string vfxPrefabName;
        [Header("仅用于编辑时: 子弹特效文件GUID")] public string vfxPrefab_GUID;

    }

}
using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public struct BulletTM {

        [Header("类型ID")] public int typeID;
        [Header("子弹名称")] public string bulletName;

        [Header("碰撞器")] public CollisionTriggerTM collisionTriggerTM;
        [Header("死亡效果器")] public int deathEffectorTypeID;

        [Header("额外穿透次数")] public int extraPenetrateCount;

        #region [子弹轨迹 ----------------------------------------------------]

        [Header("轨迹类型")] public TrajectoryType trajectoryType;

        #region [直线]

        [Header("仅用于编辑时: 位移(cm)")] public int moveDistance_cm;
        [Header("仅用于编辑时: 位移时间(帧)")] public int moveTotalFrame;
        [Header("速度组(cm)")] public int[] moveSpeedArray_cm;
        [Header("方向组")] public Vector3Int[] moveDirArray;

        #endregion

        [Header("实体追踪模型")] public EntityTrackTM entityTrackTM;

        #endregion

        [Header("仅用于编辑时: 位移曲线KeyFrameTM")] public KeyframeTM[] disCurve_KeyframeTMArray;

        [Header("子弹特效")] public string vfxPrefabName;
        [Header("仅用于编辑时: 子弹特效文件GUID")] public string vfxPrefab_GUID;

    }

}
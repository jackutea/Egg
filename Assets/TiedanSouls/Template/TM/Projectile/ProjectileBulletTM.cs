using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct ProjectileBulletTM {

        [Header("额外打击次数")] public int extraHitTimes;
        [Header("本地偏移: 位置(cm)")] public Vector3Int localPos;
        [Header("本地角度")] public Vector3Int localEulerAngles;
        [Header("子弹类型ID")] public int bulletTypeID;

    }

}
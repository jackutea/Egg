using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct ProjectileBulletTM {

        [Header("子弹类型ID")] public int bulletTypeID;
        [Header("触发帧")] public int startFrame;
        [Header("本地坐标(cm)")] public Vector3Int localPos_cm;
        [Header("本地角度")] public Vector3Int localEulerAngles;

    }

}
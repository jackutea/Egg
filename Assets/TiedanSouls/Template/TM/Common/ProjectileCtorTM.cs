using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct ProjectileCtorTM {

        [Header("类型ID")] public int typeID;
        [Header("位置偏移")] public Vector3Int localPosExpanded;
        [Header("角度偏移")] public Vector3Int localEulerAnglesExpanded;

    }

}
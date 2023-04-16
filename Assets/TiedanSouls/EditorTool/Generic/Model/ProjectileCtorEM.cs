using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    // TODO: Barrage
    [Serializable]
    public struct ProjectileCtorEM {

        [Header("类型ID")] public int typeID;
        [Header("位置偏移")] public Vector3 localPos;
        [Header("角度偏移")] public Vector3 localEulerAngles;

    }

}
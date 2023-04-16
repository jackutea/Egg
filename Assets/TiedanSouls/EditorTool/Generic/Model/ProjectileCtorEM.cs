using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct ProjectileCtorEM {

        [Header("类型ID")] public int typeID;
        [Header("初始位置偏移")] public Vector3 localOffset;

    }

}
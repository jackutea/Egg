using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct BuffAttachEM {

        [Header("类型ID")] public int typeID;
        [Header("作用阵营")] public TargetGroupType targetGroupType;

    }

}
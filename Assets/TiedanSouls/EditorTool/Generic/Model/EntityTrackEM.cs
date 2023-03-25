using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct EntityTrackEM {

        [Header("跟踪速度")] public float trackSpeed;
        [Header("跟踪目标群体")] public RelativeTargetGroupType trackTargetGroupType;
        [Header("实体跟踪 选择器")] public EntityTrackSelectorEM entityTrackSelectorEM;

    }

}
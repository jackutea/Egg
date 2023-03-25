using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct EntityTrackTM {

        [Header("跟踪速度(cm)")] public int trackSpeed_cm;
        [Header("跟踪目标群体")] public RelativeTargetGroupType trackTargetGroupType;
        [Header("实体跟踪 选择器")] public EntityTrackSelectorTM entityTrackSelectorTM;

    }

}
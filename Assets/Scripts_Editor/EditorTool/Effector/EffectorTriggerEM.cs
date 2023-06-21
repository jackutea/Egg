using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 效果器触发器
    /// </summary>
    [Serializable]
    public struct EffectorTriggerEM {

        [Header("触发帧")] public int triggerFrame;
        [Header("效果器类型")] public EffectorType effectorType;
        [Header("类型ID")] public int effectorTypeID;

    }

}
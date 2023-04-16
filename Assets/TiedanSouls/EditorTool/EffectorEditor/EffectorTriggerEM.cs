using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 效果器触发器
    /// </summary>
    [Serializable]
    public struct EffectorTriggerEM {

        [Header("触发效果器类型ID")] public int effectorTypeID;
        [Header("触发帧")] public int triggerFrame;

    }

}
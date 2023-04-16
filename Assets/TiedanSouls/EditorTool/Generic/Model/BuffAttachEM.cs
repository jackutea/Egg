using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct BuffAttachEM {

        [Header("触发帧")] public int triggerFrame;
        [Header("BuffID")] public int buffID;

    }

}
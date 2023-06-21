using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public struct BuffAttachTM {

        [Header("触发帧")] public int triggerFrame;
        [Header("类型ID")] public int buffID;

    }

}
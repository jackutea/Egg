using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct EffectorTriggerTM {

        [Header("触发帧")] public int triggerFrame;
        [Header("效果器类型")] public EffectorType effectorType;
        [Header("类型ID")] public int effectorTypeID;

    }

}
using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct EffectorTriggerTM {

        [Header("触发帧")] public int triggerFrame;
        [Header("效果器ID")] public int effectorTypeID;

    }

}
using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct KnockBackTM {

        [Header("击退帧速度(组)")] public int[] knockBackSpeedArray_cm;
        [Header("(仅用于编辑时) 击退 KeyFrameTM")] public KeyframeTM[] knockBackDisCurve_KeyframeTMArray;
        [Header("(仅用于编辑时) 击退耗时(单位: 帧)")] public int knockBackCostFrame;
        [Header("(仅用于编辑时) 击退距离(单位: cm)")] public int knockBackDistance_cm;

    }

}
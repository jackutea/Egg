using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct KnockUpPowerTM {

        [Header("击飞帧速度(组)")] public int[] knockUpSpeedArray_cm;
        [Header("(仅用于编辑时) 击飞 KeyFrameTM")] public KeyframeTM[] knockUpDisCurve_KeyframeTMArray;
        [Header("(仅用于编辑时) 击飞耗时(单位: 帧)")] public int knockUpCostFrame;
        [Header("(仅用于编辑时) 击飞距离(单位: cm)")] public int knockUpHeight_cm;

    }

}
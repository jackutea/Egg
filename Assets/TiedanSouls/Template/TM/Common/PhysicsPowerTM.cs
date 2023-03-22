using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct PhysicsPowerTM {

        [Header("击退帧速度(组)")] public int[] knockBackSpeedArray_cm;
        [Header("(仅用于编辑时) 击退 KeyFrameTM")] public KeyframeTM[] knockBackDisCurve_KeyframeTMArray;
        [Header("(仅用于编辑时) 击退耗时(单位: 帧)")] public int knockBackCostFrame;
        [Header("(仅用于编辑时) 击退距离(单位: cm)")] public int knockBackDistance_cm;

        [Header("击飞帧速度(组)")] public int[] knockUpSpeedArray_cm;
        [Header("(仅用于编辑时) 击飞 KeyFrameTM")] public KeyframeTM[] knockUpDisCurve_KeyframeTMArray;
        [Header("(仅用于编辑时) 击飞耗时(单位: 帧)")] public int knockUpCostFrame;
        [Header("(仅用于编辑时) 击飞距离(单位: cm)")] public int knockUpHeight_cm;

    }

}
using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct BeHitTM {

        [Header("持续帧数")] public int maintainFrame;

        [Header("击退帧速度(组)")] public int[] knockBackSpeedArray_cm;
        [Header("(仅用于编辑时) 击退 KeyFrameTM")] public KeyframeTM[] knockBackKeyframeTMArray;
        [Header("(仅用于编辑时) 击退耗时(单位: 帧)")] public int knockBackTotalFrame;
        [Header("(仅用于编辑时) 击退距离(单位: cm)")] public int knockBackDistance_cm;

        [Header("击飞帧速度(组)")] public int[] knockUpSpeedArray_cm;
        [Header("(仅用于编辑时) 击飞 KeyFrameTM")] public KeyframeTM[] knockUpKeyframeTMArray;
        [Header("(仅用于编辑时) 击飞耗时(单位: 帧)")] public int knockUpTotalFrame;
        [Header("(仅用于编辑时) 击飞距离(单位: cm)")] public int knockUpDis_cm;

    }

}
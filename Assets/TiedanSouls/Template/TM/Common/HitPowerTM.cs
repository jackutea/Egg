using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class HitPowerTM {

        public int startFrame;
        public int endFrame;

        [Header("击打顿帧(组)")] public int[] hitStunFrameArray;
        [Header("击打伤害(组)")] public int[] damageArray;
        [Header("击退帧速度(组)")] public int[] knockBackSpeedArray_cm;
        [Header("击飞帧速度(组)")] public int[] knockUpSpeedArray_cm;

        [Header("(仅用于编辑时) 顿帧 KeyFrameTM")] public KeyframeTM[] hitStunFrameCurve_KeyframeTMArray;
        [Header("(仅用于编辑时) 伤害 KeyFrameTM")] public KeyframeTM[] damageCurve_KeyframeTMArray;
        [Header("(仅用于编辑时) 击退 KeyFrameTM")] public KeyframeTM[] knockBackDisCurve_KeyframeTMArray;
        [Header("(仅用于编辑时) 击飞 KeyFrameTM")] public KeyframeTM[] knockUpDisCurve_KeyframeTMArray;

        [Header("(仅用于编辑时) 基础顿帧")] public int hitStunFrameBase;
        [Header("(仅用于编辑时) 基础伤害")] public int damageBase;
        [Header("(仅用于编辑时) 击退耗时(单位: 帧)")] public int knockBackCostFrame;
        [Header("(仅用于编辑时) 击退距离(单位: cm)")] public int knockBackDistance_cm;
        [Header("(仅用于编辑时) 击飞耗时(单位: 帧)")] public int knockUpCostFrame;
        [Header("(仅用于编辑时) 击飞距离(单位: cm)")] public int knockUpHeight_cm;


    }

}
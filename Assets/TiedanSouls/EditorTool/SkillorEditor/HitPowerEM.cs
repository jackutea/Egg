using System;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillorEditor {

    [Serializable]
    public class HitPowerEM {

        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("基础伤害")] public int baseDamage;
        [Header("基础伤害 - 曲线")] public AnimationCurve damageCurve;

        [Header("基础硬直帧")] public int baseHitStunFrame;
        [Header("基础硬直帧 - 曲线")] public AnimationCurve hitStunFrameCurve;

        // 击退
        [Header("击退距离")] public int knockBackDistance_cm;
        [Header("击退帧数")] public int knockBackFrame;
        [Header("击退位移曲线")] public AnimationCurve knockBackDisCurve;

        // 击飞
        [Header("击飞高度")] public int knockUpHeight_cm;
        [Header("击飞帧数")] public int knockUpFrame;
        [Header("击飞位移曲线")] public AnimationCurve knockUpDisCurve;

    }

}
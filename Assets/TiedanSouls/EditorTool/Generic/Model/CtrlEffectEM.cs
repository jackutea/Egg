using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 控制效果模型
    /// </summary>
    [Serializable]
    public struct CtrlEffectEM {

        [Header("控制效果")] public RoleCtrlEffectType ctrlEffectType;
        [Header("总帧数")] public int totalFrame;
        [Header("Icon")] public Sprite icon;

    }

}
using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    /// <summary>
    /// 角色控制效果模型
    /// </summary>
    [Serializable]
    public struct RoleCtrlEffectTM {

        [Header("控制效果")] public RoleCtrlEffectType roleCtrlEffectType;
        [Header("总帧数")] public int totalFrame;
        [Header("Icon")] public string iconName;
        [Header("Icon: GUID")] public string iconGUID;

    }

}
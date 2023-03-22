using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct StateEffectTM {

        [Header("影响状态类型")] public StateType effectStateType;
        [Header("影响状态值")] public int effectStateValue;
        [Header("影响持续时间(帧)")] public int effectMaintainFrame;

    }

}
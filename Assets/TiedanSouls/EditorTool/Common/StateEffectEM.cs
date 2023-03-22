using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool.SkillEditor {

    [Serializable]
    public struct StateEffectEM {

        [Header("影响状态类型")] public StateFlag effectStateType;
        [Header("影响状态值")] public int effectStateValue;
        [Header("影响持续时间(帧)")] public int effectMaintainFrame;

    }

}
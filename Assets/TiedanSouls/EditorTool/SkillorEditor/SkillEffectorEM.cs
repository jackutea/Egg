using System;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    [Serializable]
    public struct SkillEffectorEM {

        [Header("触发帧")] public int triggerFrame;
        [Header("触发效果器类型ID")] public int effectorTypeID;
        [Header("偏移位置")] public Vector3 offsetPos;

    }

}
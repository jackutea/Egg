using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct EffectorTM {

        [Header("类型ID")] public int typeID;
        [Header("名称")] public string effectorName;
        [Header("角色效果器")] public RoleEffectorTM roleEffectorTM;
        [Header("技能效果器")] public SkillEffectorTM skillEffectorTM;

    }

}
using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct EffectorEM {

        [Header("类型ID")] public int typeID;
        [Header("名称")] public string effectorName;
        [Header("角色选择")] public RoleEffectorEM roleEffectorEM;
        [Header("角色修改")] public SkillEffectorEM skillEffectorEM;

    }

}
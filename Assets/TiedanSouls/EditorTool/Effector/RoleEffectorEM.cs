using System;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct RoleEffectorEM {

        [Header("技能选择")] public RoleSelectorEM roleSelectorEM;
        [Header("技能修改")] public RoleModifyEM roleModifyEM;

    }

}
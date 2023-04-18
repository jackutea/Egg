using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct RoleEffectorEM {

        [Header("类型ID")] public int typeID;
        [Header("名称")] public string effectorName;
        [Header("属性满足条件")] public RoleAttributeSelectorEM roleAttributeSelectorEM;
        [Header("属性修改")] public RoleAttributeModifyEM roleAttributeModifyEM;

    }

}
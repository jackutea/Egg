using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct RoleEffectorTM {

        [Header("类型ID")] public int typeID;
        [Header("名称")] public string effectorName;
        [Header("属性满足条件")] public RoleAttributeSelectorTM roleAttributeSelectorTM;
        [Header("属性影响")] public RoleAttributeEffectTM roleAttributeEffectTM;

    }

}
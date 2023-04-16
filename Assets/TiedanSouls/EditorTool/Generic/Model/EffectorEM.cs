using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct EffectorEM {

        [Header("类型ID")] public int typeID;
        [Header("效果器名称")] public string effectorName;
        [Header("角色召唤组")] public RoleSummonEM[] roleSummonEMArray;
        [Header("Buff附加组")] public BuffAttachEM[] buffAttachEMArray;
        [Header("弹幕构造组")] public ProjectileCtorEM[] projectileCtorEMArray;
        [Header("实体销毁模型(组)")] public EntityModifyEM[] entityDestroyEMArray;

    }

}
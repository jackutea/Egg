using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct EffectorTM {

        [Header("类型ID")] public int typeID;
        [Header("效果器名称")] public string effectorName;
        [Header("角色召唤组")] public RoleSummonTM[] roleSummonTMArray;
        [Header("Buff附加组")] public BuffAttachTM[] buffAttachTMArray;
        [Header("弹幕构造组")] public ProjectileCtorTM[] projectileCtorTMArray;
        [Header("实体销毁模型(组)")] public EntityModifyTM[] entityDestroyTMArray;

    }

}
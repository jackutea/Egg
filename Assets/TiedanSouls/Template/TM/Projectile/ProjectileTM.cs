using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct ProjectileTM {

        [Header("类型ID")] public int typeID;
        [Header("弹道名称")] public string projectileName;

        [Header("根元素")] public ProjectileElementTM rootElementTM;
        [Header("叶元素")] public ProjectileElementTM[] leafElementTMArray;

    }

}
using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct ProjectileTM {

        [Header("类型ID")] public int typeID;
        [Header("名称")] public string projectileName;
        [Header("弹道子弹(组)")] public ProjectileBulletTM[] projetileBulletTMArray;

    }

}
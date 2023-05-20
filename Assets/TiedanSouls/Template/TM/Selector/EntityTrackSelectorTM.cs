using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    /// <summary>
    /// ComparisonType 为 None 时，不进行对比,即不在条件中
    /// </summary>
    [Serializable]
    public struct EntityTrackSelectorTM {

        [Header("实体类型")] public EntityType entityType;

        [Header("属性选择器是否启用")] public bool isAttributeSelectorEnabled;
        [Header("属性选择器")] public RoleSelectorTM attributeSelectorTM;

    }

}
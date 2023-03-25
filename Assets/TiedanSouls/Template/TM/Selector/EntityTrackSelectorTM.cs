using System;
using UnityEngine;

namespace TiedanSouls.Template {

    /// <summary>
    /// ComparisonType 为 None 时，不进行对比,即不在条件中
    /// </summary>
    [Serializable]
    public struct EntityTrackSelectorTM {

        [Header("属性选择器")] public AttributeSelectorTM attributeSelectorTM;

    }

}
using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// 跟踪选择器
    /// </summary>
    [Serializable]
    public struct EntityTrackSelectorEM {

        [Header("实体类型")] public EntityType entityType;

        [Header("属性选择器是否启用")] public bool isAttributeSelectorEnabled;
        [Header("属性选择器")] public AttributeSelectorEM attributeSelectorEM;

    }

}
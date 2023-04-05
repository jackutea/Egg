using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct EntityDestroyEM {

        [Header("销毁实体类型")] public EntityType entityType;
        [Header("目标类型")] public TargetGroupType hitTargetGroupType;

        [Header("启用/关闭 属性选择器")] public bool attributeSelector_IsEnabled;
        [Header("属性选择器")] public AttributeSelectorEM attributeSelectorEM;

    }

}
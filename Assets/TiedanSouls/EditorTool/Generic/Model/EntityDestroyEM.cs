using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct EntityDestroyEM {

        [Header("销毁实体类型")] public EntityType entityType;
        [Header("目标类型")] public TargetGroupType targetGroupType;

        [Header("启用/关闭 属性选择器")] public bool isEnabled_attributeSelector;
        [Header("属性选择器")] public AttributeSelectorEM attributeSelectorEM;

    }

}
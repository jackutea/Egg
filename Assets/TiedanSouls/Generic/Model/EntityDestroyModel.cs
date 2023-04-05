using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    // Use for Entity Spawn.
    [Serializable]
    public struct EntityDestroyModel {

        public EntityType entityType;
        public TargetGroupType hitTargetGroupType;

        public bool isEnabled_attributeSelector;
        public AttributeSelectorModel attributeSelectorModel;

    }

}
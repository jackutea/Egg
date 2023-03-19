using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    // Use for Entity Spawn.
    [Serializable]
    public struct EntityDestroyModel {

        public EntityType entityType;
        public TargetType targetType;

        public bool isEnabled_attributeSelector;
        public AttributeSelectorModel attributeSelectorModel;

    }

}
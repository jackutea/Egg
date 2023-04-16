using System;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    // Use for Entity Spawn.
    [Serializable]
    public struct EntityModifyModel {

        public EntityType entityType;
        public TargetGroupType hitTargetGroupType;

        public bool isEnabled_attributeSelector;
        public AttributeSelectorModel attributeSelectorModel;

    }

}
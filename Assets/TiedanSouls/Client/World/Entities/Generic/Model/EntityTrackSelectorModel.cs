using System;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// ComparisonType 为 None 时，不进行对比,即不在条件中
    /// </summary>
    [Serializable]
    public struct EntityTrackSelectorModel {

        public EntityType entityType;
        public bool isAttributeSelectorEnabled;
        public RoleAttributeSelectorModel attributeSelectorModel;

    }

}
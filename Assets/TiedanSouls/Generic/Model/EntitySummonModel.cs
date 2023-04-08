using System;

namespace TiedanSouls.Generic {

    [Serializable]
    public struct EntitySummonModel {

        public EntityType entityType;
        public int typeID;
        public ControlType controlType;

        public override string ToString() {
            return $"实体召唤实体类型 {entityType}, 类型ID {typeID}, 控制类型 {controlType}";
        }

    }

}
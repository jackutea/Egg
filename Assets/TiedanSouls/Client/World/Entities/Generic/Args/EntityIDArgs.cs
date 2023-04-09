using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public struct EntityIDArgs {

        public EntityType entityType;
        public short entityID;
        public int typeID;
        public string entityName;
        public AllyType allyType;
        public ControlType controlType;

        public int fromFieldTypeID;

        public bool IsTheSameAs(in EntityIDArgs other) {
            return entityType == other.entityType
                && typeID == other.typeID
                && entityID == other.entityID;
        }

        public override string ToString() {
            return $"IDArgs: 实体类型 {entityType} 类型ID {typeID} 实体ID {entityID} 实体名称 {entityName} 阵营 {allyType} 控制类型 {controlType} 来自关卡 {fromFieldTypeID}";
        }

    }

}
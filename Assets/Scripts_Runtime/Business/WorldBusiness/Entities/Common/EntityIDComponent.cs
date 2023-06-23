using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 实体ID组件
    /// </summary>
    public class EntityIDComponent {

        object holderPtr;
        public object HolderPtr => holderPtr;
        public void SetHolderPtr(object value) => this.holderPtr = value;

        EntityType entityType;
        public EntityType EntityType => entityType;
        public void SetEntityType(EntityType value) => this.entityType = value;

        short entityID;
        public short EntityID => entityID;
        public void SetEntityID(short value) => this.entityID = value;

        int typeID;
        public int TypeID => typeID;
        public void SetTypeID(int value) => this.typeID = value;

        string entityName;
        public string EntityName => entityName;
        public void SetEntityName(string name) => this.entityName = name;

        CampType campType;
        public CampType AllyStatus => campType;
        public void SetAllyType(CampType value) => this.campType = value;

        ControlType controlType;
        public ControlType ControlType => controlType;
        public void SetControlType(ControlType value) => this.controlType = value;

        int fromFieldTypeID;
        public int FromFieldTypeID => fromFieldTypeID;
        public void SetFromFieldTypeID(int value) => this.fromFieldTypeID = value;

        EntityIDArgs father;
        public EntityIDArgs Father => father;

        public EntityIDComponent() {
            Reset();
        }

        public void Reset() {

        }

        public bool IsEqualTo(EntityIDComponent other) {
            return entityType == other.entityType
                && typeID == other.typeID
                && entityID == other.entityID;
        }

        /// <summary>
        /// 设置父级
        /// </summary>
        public void SetFather(in EntityIDArgs args) {
            this.father = args;

            this.campType = args.campType;
            this.fromFieldTypeID = args.fromFieldTypeID;

        }

        public EntityIDArgs ToArgs() {
            EntityIDArgs args = new EntityIDArgs();
            args.entityType = entityType;
            args.typeID = typeID;
            args.entityID = entityID;
            args.entityName = entityName;
            args.campType = campType;
            args.controlType = controlType;
            args.fromFieldTypeID = fromFieldTypeID;

            return args;
        }

        public override string ToString() {
            return $"IDComponent: 实体类型 {entityType} 类型ID {typeID} 实体ID {entityID} 实体名称 {entityName} 阵营 {campType} 控制类型 {controlType} 来自关卡 {fromFieldTypeID}";
        }

    }

}
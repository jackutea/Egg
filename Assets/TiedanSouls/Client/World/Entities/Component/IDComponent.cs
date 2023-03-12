namespace TiedanSouls.World.Entities {

    public class IDComponent {

        // ==== Identity ====
        public EntityType EntityType => EntityType.Role;

        int entityID;
        public int EntityID => entityID;
        public void SetEntityD(int value) => this.entityID = value;

        int typeID;
        public int TypeID => typeID;
        public void SetTypeID(int value) => this.typeID = value;

        string entityName;
        public string EntityName => entityName;
        public void SetEntityName(string name) => this.entityName = name;

        AllyType allyType;
        public AllyType AllyType => allyType;
        public void SetAlly(AllyType value) => this.allyType = value;

    }

}
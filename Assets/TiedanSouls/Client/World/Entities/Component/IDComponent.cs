namespace TiedanSouls.World.Entities {

    public struct IDArgs {
        public EntityType entityType;
        public int entityID;
        public int typeID;
        public string entityName;
        public AllyType allyType;
    }

    public class IDComponent {

        // ==== Identity ====
        EntityType entityType;
        public EntityType EntityType => entityType;
        public void SetEntityType(EntityType value) => this.entityType = value;

        int entityID;
        public int EntityID => entityID;
        public void SetEntityID(int value) => this.entityID = value;

        int typeID;
        public int TypeID => typeID;
        public void SetTypeID(int value) => this.typeID = value;

        string entityName;
        public string EntityName => entityName;
        public void SetEntityName(string name) => this.entityName = name;

        AllyType allyType;
        public AllyType AllyType => allyType;
        public void SetAlly(AllyType value) => this.allyType = value;

        IDArgs father;
        public IDArgs Father => father;
        public void SetFather(IDArgs value) => this.father = value;

        public IDArgs ToArgs() {
            IDArgs args = new IDArgs();
            args.entityType = entityType;
            args.entityID = entityID;
            args.typeID = typeID;
            args.entityName = entityName;
            args.allyType = allyType;
            return args;
        }

    }

}
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World.Domain {

    public class WorldFieldDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldFieldDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public FieldEntity SpawnField(int typeID) {
            // Field
            var field = worldContext.WorldFactory.SpawnFieldEntity(typeID);
            var fieldRepo = worldContext.FieldRepo;
            fieldRepo.Add(field);
            return field;
        }

    }
}
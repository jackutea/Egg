using TiedanSouls.Asset;
using TiedanSouls.World.Entities;
using TiedanSouls.World.Service;

namespace TiedanSouls.World.Facades {

    public class WorldContext {

        // ==== State ====
        WorldStateEntity stateEntity;
        public WorldStateEntity StateEntity => stateEntity;

        // ==== Factory ====
        WorldFactory worldFactory;
        public WorldFactory WorldFactory => worldFactory;

        // ==== Services ====
        IDService idService;
        public IDService IDService => idService;

        DamageArbitService damageArbitService;
        public DamageArbitService DamageArbitService => damageArbitService;

        // ==== Repository ====
        RoleRepo roleRepo;
        public RoleRepo RoleRepo => roleRepo;

        FieldRepo fieldRepo;
        public FieldRepo FieldRepo => fieldRepo;

        ItemRepo itemRepo;
        public ItemRepo ItemRepo => itemRepo;

        SkillRepo skillRepo;
        public SkillRepo SkillRepo => skillRepo;

        public WorldDomain WorldDomain { get; private set; }

        public WorldContext() {
            stateEntity = new WorldStateEntity();

            worldFactory = new WorldFactory();

            idService = new IDService();
            damageArbitService = new DamageArbitService();

            roleRepo = new RoleRepo();
            fieldRepo = new FieldRepo();
            itemRepo = new ItemRepo();
            skillRepo = new SkillRepo();
        }

        public void Inject(WorldDomain worldDomain) {
            this.WorldDomain = worldDomain;
        }

    }

}
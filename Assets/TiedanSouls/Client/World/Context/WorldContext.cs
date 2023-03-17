using TiedanSouls.Client.Entities;
using TiedanSouls.Client.Service;

namespace TiedanSouls.Client.Facades {

    public class WorldContext {

        // ==== State ====
        public WorldStateEntity StateEntity { get; private set; }

        // ==== Factory ====
        public WorldFactory WorldFactory { get; private set; }

        // ==== Services ====
        public IDService IDService { get; private set; }
        public DamageArbitService DamageArbitService { get; private set; }

        // ==== Repository ====
        public RoleRepo RoleRepo { get; private set; }
        public SkillRepo SkillRepo { get; private set; }
        public FieldRepo FieldRepo { get; private set; }
        public ItemRepo ItemRepo { get; private set; }
        public CollisionEventRepo CollisionEventRepo { get; private set; }

        // ==== Domain ====
        public WorldRootDomain RootDomain { get; private set; }

        public WorldContext() {
            StateEntity = new WorldStateEntity();

            WorldFactory = new WorldFactory();

            IDService = new IDService();
            DamageArbitService = new DamageArbitService();

            RoleRepo = new RoleRepo();
            SkillRepo = new SkillRepo();
            FieldRepo = new FieldRepo();
            ItemRepo = new ItemRepo();
            CollisionEventRepo = new CollisionEventRepo();
        }

        public void Inject(WorldRootDomain rootDomain) {
            this.RootDomain = rootDomain;
        }

    }

}
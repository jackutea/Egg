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
        RoleRepository roleRepo;
        public RoleRepository RoleRepo => roleRepo;

        FieldRepository fieldRepo;
        public FieldRepository FieldRepo => fieldRepo;

        public WorldContext() {
            
            stateEntity = new WorldStateEntity();
            
            worldFactory = new WorldFactory();
            
            idService = new IDService();
            damageArbitService = new DamageArbitService();
            
            roleRepo = new RoleRepository();
            fieldRepo = new FieldRepository();

        }

    }

}
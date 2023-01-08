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

        // ==== Repository ====
        RoleRepository roleRepo;
        public RoleRepository RoleRepo => roleRepo;

        public WorldContext() {
            
            stateEntity = new WorldStateEntity();
            
            worldFactory = new WorldFactory();
            
            idService = new IDService();
            
            roleRepo = new RoleRepository();

        }

    }

}
using TiedanSouls.World.Domain;

namespace TiedanSouls.World.Facades {

    public class WorldDomain {

        WorldFSMDomain gameDomain;
        public WorldFSMDomain GameDomain => gameDomain;

        WorldFieldDomain fieldDomain;
        public WorldFieldDomain FieldDomain => fieldDomain;

        WorldRoleDomain roleDomain;
        public WorldRoleDomain RoleDomain => roleDomain;

        WorldRoleFSMDomain roleFSMDomain;
        public WorldRoleFSMDomain RoleFSMDomain => roleFSMDomain;   

        WorldPhysicsDomain phxDomain;
        public WorldPhysicsDomain WorldPhysicsDomain => phxDomain;

        public WorldDomain() {
            gameDomain = new WorldFSMDomain();
            fieldDomain = new WorldFieldDomain();
            roleDomain = new WorldRoleDomain();
            roleFSMDomain = new WorldRoleFSMDomain();
            phxDomain = new WorldPhysicsDomain();
        }

    }
}
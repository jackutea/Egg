using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Domain;

namespace TiedanSouls.World.Facades {

    public class WorldDomain {

        WorldFSMDomain gameDomain;
        public WorldFSMDomain GameDomain => gameDomain;

        WorldFieldDomain fieldDomain;
        public WorldFieldDomain FieldDomain => fieldDomain;

        WorldFieldFSMDomain fieldFSMDomain;
        public WorldFieldFSMDomain FieldFSMDomain => fieldFSMDomain;

        WorldRoleDomain roleDomain;
        public WorldRoleDomain RoleDomain => roleDomain;

        WorldRoleFSMDomain roleFSMDomain;
        public WorldRoleFSMDomain RoleFSMDomain => roleFSMDomain;

        WorldPhysicsDomain phxDomain;
        public WorldPhysicsDomain WorldPhysicsDomain => phxDomain;

        WorldRendererDomain worldRendererDomain;
        public WorldRendererDomain WorldRendererDomain => worldRendererDomain;

        public WorldDomain() {
            gameDomain = new WorldFSMDomain();

            fieldDomain = new WorldFieldDomain();
            fieldFSMDomain = new WorldFieldFSMDomain();

            roleDomain = new WorldRoleDomain();
            roleFSMDomain = new WorldRoleFSMDomain();

            phxDomain = new WorldPhysicsDomain();

            worldRendererDomain = new WorldRendererDomain();
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            GameDomain.Inject(infraContext, worldContext, this);

            RoleFSMDomain.Inject(infraContext, worldContext, this);
            RoleDomain.Inject(infraContext, worldContext);

            FieldDomain.Inject(infraContext, worldContext);
            FieldFSMDomain.Inject(infraContext, worldContext);

            WorldPhysicsDomain.Inject(infraContext, worldContext, this);

            WorldRendererDomain.Inject(infraContext, worldContext, this);
        }

    }
}
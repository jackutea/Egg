using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client.Domain {

    public class WorldProjectileElementDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldRootDomain;

        public WorldProjectileElementDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldRootDomain = worldDomain;
        }

        public void TickElement(ProjectileElement element, float dt) {
        }

    }

}
using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;

namespace TiedanSouls.Client.Domain {

    public class WorldPhysicsDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldDomain;

        public WorldPhysicsDomain() { 
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void Tick(float dt) {

            Physics2D.Simulate(dt);

        }

    }

}
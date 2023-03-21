using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Domain {

    public class WorldProjectileElementFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldRootDomain;

        public WorldProjectileElementFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldRootDomain = worldDomain;
        }

    }

}
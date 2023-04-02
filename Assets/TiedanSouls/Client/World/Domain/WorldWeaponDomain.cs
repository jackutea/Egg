using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldWeaponDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldWeaponDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain rootDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = rootDomain;
        }

        #region [添加武器]

        #endregion

    }

}
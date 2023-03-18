using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldHitDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldRootDomain;

        public WorldHitDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldRootDomain = worldDomain;
        }

        public void HitRoleByHitPower(RoleEntity role, HitPowerModel hitPower, int hitFrame, Vector3 beHitDir) {
            var hitDamage = hitPower.GetHitDamage(hitFrame);
            role.Attribute_HP_Decrease(hitDamage);

            role.FSMCom.EnterBeHit(hitPower, hitFrame, beHitDir);
        }

    }

}
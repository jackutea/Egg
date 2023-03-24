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

        public void Role_BeHit(RoleEntity role, in CollisionTriggerModel collisionTriggerModel, int hitFrame, Vector2 beHitDir) {
            var fsm = role.FSMCom;

            // 击退
            var knockBackPowerModel = collisionTriggerModel.knockBackPowerModel;
            fsm.AddKnockBack(beHitDir, knockBackPowerModel);
            // 击飞
            var knockUpPowerModel = collisionTriggerModel.knockUpPowerModel;
            fsm.AddKnockUp(knockUpPowerModel);

            // 伤害结算
            var damageModel = collisionTriggerModel.damageModel;
            var hitDamage = damageModel.GetDamage(hitFrame - collisionTriggerModel.totalFrame);
            role.Attribute_HP_Decrease(hitDamage);
        }

    }

}
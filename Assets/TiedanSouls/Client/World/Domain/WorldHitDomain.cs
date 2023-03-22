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
            // 伤害结算
            var damageModel = collisionTriggerModel.damageModel;
            var hitDamage =damageModel.GetDamage(hitFrame);
            role.Attribute_HP_Decrease(hitDamage);

            // 物理力度
            var physicsPowerModel = collisionTriggerModel.physicsPowerModel;
            // 状态机切换
            role.FSMCom.EnterBeHit(physicsPowerModel, hitFrame, beHitDir);
        }

    }

}
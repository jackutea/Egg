using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client.Domain {

    public class WorldPhysicsDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldRootDomain;

        public WorldPhysicsDomain() {
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldRootDomain = worldDomain;
        }

        public void Tick(float dt) {
            Physics2D.Simulate(dt);

            var collisionEventRepo = worldContext.CollisionEventRepo;
            while (collisionEventRepo.TryPick_EnterEvent(out var ev)) {
                var idArgs1 = ev.A;
                var idArgs2 = ev.B;
                _ = worldRootDomain.TryGetEntityObj(idArgs1, out var entity1);
                _ = worldRootDomain.TryGetEntityObj(idArgs2, out var entity2);
                TriggerEnter(idArgs1, entity1, idArgs2, entity2);
            }

            while (collisionEventRepo.TryPick_ExitEvent(out var ev)) {
                var idArgs1 = ev.A;
                var idArgs2 = ev.B;
                _ = worldRootDomain.TryGetEntityObj(idArgs1, out var entity1);
                _ = worldRootDomain.TryGetEntityObj(idArgs2, out var entity2);
                TriggerExit(idArgs1, entity1, idArgs2, entity2);
            }
        }

        void TriggerEnter(in IDArgs argsA, IEntity entityA, in IDArgs argsB, IEntity entityB) {
            if (argsA.entityType == EntityType.Role && argsB.entityType == EntityType.Skill)
                TriggerEnter_SkillNRole((SkillEntity)entityB, (RoleEntity)entityA);
            else if (argsA.entityType == EntityType.Skill && argsB.entityType == EntityType.Role)
                TriggerEnter_SkillNRole((SkillEntity)entityA, (RoleEntity)entityB);
        }

        void TriggerEnter_SkillNRole(SkillEntity skillEntity, RoleEntity roleEntity) {
            TDLog.Log($"碰撞事件<Trigger - Enter>:\n{skillEntity.IDCom}\n{roleEntity.IDCom.EntityID}");
        }

        void TriggerExit(in IDArgs argsA, IEntity entityA, in IDArgs argsB, IEntity entityB) {
            if (argsA.entityType == EntityType.Role && argsB.entityType == EntityType.Skill)
                TriggerExit_SkillNRole((SkillEntity)entityB, (RoleEntity)entityA);
            else if (argsA.entityType == EntityType.Skill && argsB.entityType == EntityType.Role)
                TriggerExit_SkillNRole((SkillEntity)entityA, (RoleEntity)entityB);
        }

        void TriggerExit_SkillNRole(SkillEntity skillEntity, RoleEntity roleEntity) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{skillEntity.IDCom}\n{roleEntity.IDCom.EntityID}");
        }

    }

}
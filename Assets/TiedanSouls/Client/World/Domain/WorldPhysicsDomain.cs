using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldPhysicsDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldPhysicsDomain() {
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = worldDomain;
        }

        public void Tick(float dt) {
            Physics2D.Simulate(dt);

            var collisionEventRepo = worldContext.CollisionEventRepo;
            while (collisionEventRepo.TryPick_Enter(out var ev)) {
                var idArgs1 = ev.A;
                var idArgs2 = ev.B;
                _ = rootDomain.TryGetEntityObj(idArgs1, out var entity1);
                _ = rootDomain.TryGetEntityObj(idArgs2, out var entity2);
                HandleTriggerEnter(entity1, entity2);
            }

            while (collisionEventRepo.TryPick_Exit(out var ev)) {
                var idArgs1 = ev.A;
                var idArgs2 = ev.B;
                _ = rootDomain.TryGetEntityObj(idArgs1, out var entity1);
                _ = rootDomain.TryGetEntityObj(idArgs2, out var entity2);
                HandleTriggerExit(entity1, entity2);
            }
        }

        #region [物理方法]


        #endregion

        #region [碰撞事件处理]

        void HandleTriggerEnter(IEntity entityA, IEntity entityB) {
            if (entityA is SkillEntity skillEntity && entityB is RoleEntity roleEntity) {
                HandleTriggerEnter_SkillNRole(skillEntity, roleEntity);
                return;
            }

            if (entityA is RoleEntity roleEntity2 && entityB is SkillEntity skillEntity2) {
                HandleTriggerEnter_SkillNRole(skillEntity2, roleEntity2);
                return;
            }

            if (entityA is RoleEntity roleEntity3 && entityB is RoleEntity roleEntity4) {
                HandleTriggerEnter_RoleNRole(roleEntity3, roleEntity4);
                return;
            }

            TDLog.Error($"未处理的碰撞事件<Trigger - Enter>:\n{entityA.IDCom}\n{entityB.IDCom}");
        }

        void HandleTriggerExit(IEntity entityA, IEntity entityB) {
            if (entityA is SkillEntity skillEntity && entityB is RoleEntity roleEntity) {
                HandleTriggerExit_SkillNRole(skillEntity, roleEntity);
                return;
            }

            if (entityA is RoleEntity roleEntity2 && entityB is SkillEntity skillEntity2) {
                HandleTriggerExit_SkillNRole(skillEntity2, roleEntity2);
                return;
            }

            if (entityA is RoleEntity roleEntity3 && entityB is RoleEntity roleEntity4) {
                HandleTriggerExit_RoleNRole(roleEntity3, roleEntity4);
                return;
            }

            TDLog.Error($"未处理的碰撞事件<Trigger - Exit>:\n{entityA.IDCom}\n{entityB.IDCom}");
        }

        void HandleTriggerEnter_SkillNRole(SkillEntity skill, RoleEntity role) {
            if (!skill.TryGet_ValidTriggerModel(out var model)) {
                return;
            }

            _ = rootDomain.TryGetEntityObj(skill.IDCom.Father, out var fatherEntity);
            var casterRole = fatherEntity as RoleEntity;
            var casterPos = casterRole.GetPos_Logic();
            var rolePos = role.GetPos_Logic();
            var beHitDir = rolePos - casterPos;
            beHitDir.Normalize();

            var hitPower = model.hitPower;
            var hitDomain = rootDomain.HitDomain;
            hitDomain.HitRoleByHitPower(role, hitPower, skill.CurFrame, beHitDir);
        }

        void HandleTriggerEnter_RoleNRole(RoleEntity role1, RoleEntity role2) {
            // TDLog.Log($"碰撞事件<Trigger - Enter>:\n{role1.IDCom}\n{role2.IDCom}");
        }

        void HandleTriggerExit_SkillNRole(SkillEntity skill, RoleEntity role) {
            // TDLog.Log($"碰撞事件<Trigger - Exit>:\n{skill.IDCom}\n{role.IDCom}");
        }

        void HandleTriggerExit_RoleNRole(RoleEntity role1, RoleEntity role2) {
            // TDLog.Log($"碰撞事件<Trigger - Exit>:\n{role1.IDCom}\n{role2.IDCom}");
        }

        #endregion

    }

}
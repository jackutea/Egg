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
                if (rootDomain.TryGetEntityObj(idArgs1, out var entity1)) continue;
                if (rootDomain.TryGetEntityObj(idArgs2, out var entity2)) continue;
                HandleTriggerExit(entity1, entity2);
            }
        }

        #region [物理方法]


        #endregion

        #region [碰撞事件处理 Enter]

        void HandleTriggerEnter(IEntity entityA, IEntity entityB) {
            // 技能 & 角色 
            if (entityA is SkillEntity skillEntity && entityB is RoleEntity roleEntity) {
                HandleTriggerEnter_Skill_Role(skillEntity, roleEntity);
                return;
            }
            if (entityA is RoleEntity roleEntity2 && entityB is SkillEntity skillEntity2) {
                HandleTriggerEnter_Skill_Role(skillEntity2, roleEntity2);
                return;
            }

            // 子弹 & 角色
            if (entityA is BulletEntity bulletEntity && entityB is RoleEntity roleEntity5) {
                HandleTriggerEnter_Bullet_Role(bulletEntity, roleEntity5);
                return;
            }
            if (entityA is RoleEntity roleEntity6 && entityB is BulletEntity bulletEntity2) {
                HandleTriggerEnter_Bullet_Role(bulletEntity2, roleEntity6);
                return;
            }

            // 子弹 & 技能
            if (entityA is BulletEntity bulletEntity3 && entityB is SkillEntity skillEntity3) {
                HandleTriggerEnter_Bullet_Skill(bulletEntity3, skillEntity3);
                return;
            }
            if (entityA is SkillEntity skillEntity4 && entityB is BulletEntity bulletEntity4) {
                HandleTriggerEnter_Bullet_Skill(bulletEntity4, skillEntity4);
                return;
            }

            // 角色 & 角色
            if (entityA is RoleEntity roleEntity3 && entityB is RoleEntity roleEntity4) {
                HandleTriggerEnter_Role_Role(roleEntity3, roleEntity4);
                return;
            }

            // 子弹 & 子弹
            if (entityA is BulletEntity bulletEntity5 && entityB is BulletEntity bulletEntity6) {
                HandleTriggerEnter_BulletNBullet(bulletEntity5, bulletEntity6);
                return;
            }

            TDLog.Error($"未处理的碰撞事件<Trigger - Enter>:\n{entityA.IDCom}\n{entityB.IDCom}");
        }

        void HandleTriggerEnter_Skill_Role(SkillEntity skill, RoleEntity role) {
            if (!skill.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            _ = rootDomain.TryGetEntityObj(skill.IDCom.Father, out var fatherEntity);
            var casterRole = fatherEntity as RoleEntity;
            var casterPos = casterRole.LogicPos;
            var rolePos = role.LogicPos;
            var beHitDir = rolePos - casterPos;
            beHitDir.Normalize();

            // 角色受击
            var roleDomain = rootDomain.RoleDomain;
            roleDomain.HandleBeHit(skill.CurFrame, beHitDir, role, skill.IDCom.ToArgs(), collisionTriggerModel);
        }

        void HandleTriggerEnter_Bullet_Role(BulletEntity bullet, RoleEntity role) {
            if (!bullet.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            var rolePos = role.LogicPos;
            var beHitDir = rolePos - bullet.LogicPos;
            beHitDir.Normalize();

            // 角色撞击事件
            var roleDomain = rootDomain.RoleDomain;
            var hitFrame = bullet.FSMCom.ActivatedModel.curFrame;
            roleDomain.HandleBeHit(hitFrame, beHitDir, role, bullet.IDCom.ToArgs(), collisionTriggerModel);

            // 子弹撞击事件
            var bulletDomain = rootDomain.BulletDomain;
            bulletDomain.HandleHit(bullet);
        }

        void HandleTriggerEnter_Bullet_Skill(BulletEntity bullet, SkillEntity skill) {
            if (!bullet.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            var skillDomain = rootDomain.SkillDomain;
            skillDomain.HandleBeHit(skill, collisionTriggerModel, bullet.FSMCom.ActivatedModel.curFrame);
        }

        void HandleTriggerEnter_Role_Role(RoleEntity role1, RoleEntity role2) {
            TDLog.Log($"碰撞事件<Trigger - Enter>:\n{role1.IDCom}\n{role2.IDCom}");
        }

        void HandleTriggerEnter_BulletNBullet(BulletEntity bullet1, BulletEntity bullet2) {
            var bulletDomain = rootDomain.BulletDomain;
            bulletDomain.HandleHit(bullet1);
            bulletDomain.HandleHit(bullet2);
            bulletDomain.HandleBeHit(bullet1);
            bulletDomain.HandleBeHit(bullet2);
        }

        #endregion

        #region [碰撞事件处理 Exit]

        void HandleTriggerExit(IEntity entityA, IEntity entityB) {
            // 技能 & 角色
            if (entityA is SkillEntity skillEntity3 && entityB is RoleEntity roleEntity) {
                HandleTriggerExit_Skill_Role(skillEntity3, roleEntity);
                return;
            }
            if (entityA is RoleEntity roleEntity2 && entityB is SkillEntity skillEntity4) {
                HandleTriggerExit_Skill_Role(skillEntity4, roleEntity2);
                return;
            }

            // 子弹 & 角色
            if (entityA is BulletEntity bulletEntity && entityB is RoleEntity roleEntity5) {
                HandleTriggerExit_Bullet_Role(bulletEntity, roleEntity5);
                return;
            }
            if (entityA is RoleEntity roleEntity6 && entityB is BulletEntity bulletEntity2) {
                HandleTriggerExit_Bullet_Role(bulletEntity2, roleEntity6);
                return;
            }

            // 子弹 & 技能
            if (entityA is BulletEntity bulletEntity3 && entityB is SkillEntity skillEntity5) {
                HandleTriggerExit_Bullet_Skill(bulletEntity3, skillEntity5);
                return;
            }
            if (entityA is SkillEntity skillEntity6 && entityB is BulletEntity bulletEntity4) {
                HandleTriggerExit_Bullet_Skill(bulletEntity4, skillEntity6);
                return;
            }

            // 角色 & 角色
            if (entityA is RoleEntity roleEntity3 && entityB is RoleEntity roleEntity4) {
                HandleTriggerExit_Role_Role(roleEntity3, roleEntity4);
                return;
            }

            // 子弹 & 子弹
            if (entityA is BulletEntity bulletEntity5 && entityB is BulletEntity bulletEntity6) {
                HandleTriggerExit_Bullet_Bullet(bulletEntity5, bulletEntity6);
                return;
            }

            // 技能 & 技能
            if (entityA is SkillEntity skillEntity && entityB is SkillEntity skillEntity2) {
                HandleTriggerExit_Skill_Skill(skillEntity, skillEntity2);
                return;
            }

            TDLog.Error($"未处理的碰撞事件<Trigger - Exit>:\n{entityA.IDCom}\n{entityB.IDCom}");
        }

        void HandleTriggerExit_Bullet_Bullet(BulletEntity bullet1, BulletEntity bullet2) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{bullet1.IDCom}\n{bullet2.IDCom}");
        }

        void HandleTriggerExit_Skill_Skill(SkillEntity skill1, SkillEntity skill2) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{skill1.IDCom}\n{skill2.IDCom}");
        }

        void HandleTriggerExit_Skill_Role(SkillEntity skill, RoleEntity role) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{skill.IDCom}\n{role.IDCom}");
        }

        void HandleTriggerExit_Role_Role(RoleEntity role1, RoleEntity role2) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{role1.IDCom}\n{role2.IDCom}");
        }

        void HandleTriggerExit_Bullet_Role(BulletEntity bullet, RoleEntity role) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{bullet.IDCom}\n{role.IDCom}");
        }

        void HandleTriggerExit_Bullet_Skill(BulletEntity bullet, SkillEntity skill) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{bullet.IDCom}\n{skill.IDCom}");
        }

        #endregion

    }

}
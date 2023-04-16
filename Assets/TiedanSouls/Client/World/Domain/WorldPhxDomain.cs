using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldPhxDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldPhxDomain() {
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public void Tick(float dt) {
            Physics2D.Simulate(dt);

            var collisionEventRepo = worldContext.CollisionEventRepo;

            collisionEventRepo.Foreach_TriggerEnter(HandleEnter);
            collisionEventRepo.Foreach_CollisionEnter(HandleEnter);

            collisionEventRepo.Foreach_TriggerStay(HandleStay);
            collisionEventRepo.Foreach_CollisionStay(HandleStay);

            collisionEventRepo.Foreach_TriggerExit(HandleExit);
            collisionEventRepo.Foreach_CollisionExit(HandleExit);

            collisionEventRepo.Update();

            var roleRepo = worldContext.RoleRepo;
            var roleFSMDomain = worldContext.RootDomain.RoleFSMDomain;
            roleRepo.Foreach_All((role) => {
                if (role.groundCount > 0) {
                    if (!role.FSMCom.PositionStatus.Contains(RolePositionStatus.OnGround)) roleFSMDomain.AddPositionStatus_OnGround(role);
                } else {
                    if (role.FSMCom.PositionStatus.Contains(RolePositionStatus.OnGround)) roleFSMDomain.RemovePositionStatus_OnGround(role);
                }
                role.groundCount = 0;
            });

        }

        void HandleEnter(in EntityCollisionEvent evModel) {
            var rootDomain = worldContext.RootDomain;
            var entityColliderModelA = evModel.entityColliderModelA;
            var entityColliderModelB = evModel.entityColliderModelB;
            var fatherA = entityColliderModelA.Father;
            var fatherB = entityColliderModelB.Father;
            if (!rootDomain.TryGetEntityObj(fatherA, out var entityA)) return;
            if (!rootDomain.TryGetEntityObj(fatherB, out var entityB)) return;

            // 技能 & 角色 
            if (entityA is SkillEntity skillEntity && entityB is RoleEntity roleEntity) {
                HandleEnter_Skill_Role(skillEntity, roleEntity, evModel);
                return;
            }
            if (entityA is RoleEntity roleEntity2 && entityB is SkillEntity skillEntity2) {
                HandleEnter_Skill_Role(skillEntity2, roleEntity2, evModel);
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
                HandleEnter_Bullet_Skill(bulletEntity3, skillEntity3);
                return;
            }
            if (entityA is SkillEntity skillEntity4 && entityB is BulletEntity bulletEntity4) {
                HandleEnter_Bullet_Skill(bulletEntity4, skillEntity4);
                return;
            }

            // 角色 & 角色
            if (entityA is RoleEntity roleEntity3 && entityB is RoleEntity roleEntity4) {
                HandleEnter_Role_Role(roleEntity3, roleEntity4);
                return;
            }

            // 子弹 & 子弹
            if (entityA is BulletEntity bulletEntity5 && entityB is BulletEntity bulletEntity6) {
                HandleEnter_BulletNBullet(bulletEntity5, bulletEntity6);
                return;
            }

            // 角色 & Field
            if (entityA is RoleEntity roleEntity7 && entityB is FieldEntity fieldEntity) {
                HandleEnter_Role_Field(roleEntity7, fieldEntity, evModel);
                return;
            }
            if (entityA is FieldEntity fieldEntity2 && entityB is RoleEntity roleEntity8) {
                HandleEnter_Role_Field(roleEntity8, fieldEntity2, evModel);
                return;
            }

            // 子弹 & Field
            if (entityA is BulletEntity bulletEntity7 && entityB is FieldEntity fieldEntity3) {
                HandleEnter_Bullet_Field(bulletEntity7, fieldEntity3);
                return;
            }
            if (entityA is FieldEntity fieldEntity4 && entityB is BulletEntity bulletEntity8) {
                HandleEnter_Bullet_Field(bulletEntity8, fieldEntity4);
                return;
            }

            TDLog.Warning($"未处理的碰撞事件<Enter>:\n{entityA.IDCom}\n{entityB.IDCom}");
        }

        void HandleStay(in EntityCollisionEvent evModel) {
            var rootDomain = worldContext.RootDomain;
            var entityColliderModelA = evModel.entityColliderModelA;
            var entityColliderModelB = evModel.entityColliderModelB;
            var fatherA = entityColliderModelA.Father;
            var fatherB = entityColliderModelB.Father;
            if (!rootDomain.TryGetEntityObj(fatherA, out var entityA)) return;
            if (!rootDomain.TryGetEntityObj(fatherB, out var entityB)) return;

            // 角色 & Field
            if (entityA is RoleEntity roleEntity && entityB is FieldEntity fieldEntity) {
                HandleStay_Role_Field(roleEntity, fieldEntity, evModel);
                return;
            }
            if (entityA is FieldEntity fieldEntity2 && entityB is RoleEntity roleEntity2) {
                HandleStay_Role_Field(roleEntity2, fieldEntity2, evModel);
                return;
            }
        }
   
        void HandleExit(in EntityCollisionEvent evModel) {
            var rootDomain = worldContext.RootDomain;
            var entityColliderModelA = evModel.entityColliderModelA;
            var entityColliderModelB = evModel.entityColliderModelB;
            var fatherA = entityColliderModelA.Father;
            var fatherB = entityColliderModelB.Father;
            if (!rootDomain.TryGetEntityObj(fatherA, out var entityA)) return;
            if (!rootDomain.TryGetEntityObj(fatherB, out var entityB)) return;

            // 技能 & 角色
            if (entityA is SkillEntity skillEntity3 && entityB is RoleEntity roleEntity) {
                HandleExit_Skill_Role(skillEntity3, roleEntity);
                return;
            }
            if (entityA is RoleEntity roleEntity2 && entityB is SkillEntity skillEntity4) {
                HandleExit_Skill_Role(skillEntity4, roleEntity2);
                return;
            }

            // 子弹 & 角色
            if (entityA is BulletEntity bulletEntity && entityB is RoleEntity roleEntity5) {
                HandleExit_Bullet_Role(bulletEntity, roleEntity5);
                return;
            }
            if (entityA is RoleEntity roleEntity6 && entityB is BulletEntity bulletEntity2) {
                HandleExit_Bullet_Role(bulletEntity2, roleEntity6);
                return;
            }

            // 子弹 & 技能
            if (entityA is BulletEntity bulletEntity3 && entityB is SkillEntity skillEntity5) {
                HandleExit_Bullet_Skill(bulletEntity3, skillEntity5);
                return;
            }
            if (entityA is SkillEntity skillEntity6 && entityB is BulletEntity bulletEntity4) {
                HandleExit_Bullet_Skill(bulletEntity4, skillEntity6);
                return;
            }

            // 角色 & 角色
            if (entityA is RoleEntity roleEntity3 && entityB is RoleEntity roleEntity4) {
                HandleExit_Role_Role(roleEntity3, roleEntity4);
                return;
            }

            // 子弹 & 子弹
            if (entityA is BulletEntity bulletEntity5 && entityB is BulletEntity bulletEntity6) {
                HandleExit_Bullet_Bullet(bulletEntity5, bulletEntity6);
                return;
            }

            // 技能 & 技能
            if (entityA is SkillEntity skillEntity && entityB is SkillEntity skillEntity2) {
                HandleExit_Skill_Skill(skillEntity, skillEntity2);
                return;
            }

            // 角色 & Field
            if (entityA is RoleEntity roleEntity7 && entityB is FieldEntity fieldEntity) {
                HandleExit_Role_Field(roleEntity7, fieldEntity, evModel);
                return;
            }
            if (entityA is FieldEntity fieldEntity2 && entityB is RoleEntity roleEntity8) {
                HandleExit_Role_Field(roleEntity8, fieldEntity2, evModel);
                return;
            }

            // 子弹 & Field
            if (entityA is BulletEntity bulletEntity7 && entityB is FieldEntity fieldEntity3) {
                HandleExit_Bullet_Field(bulletEntity7, fieldEntity3);
                return;
            }
            if (entityA is FieldEntity fieldEntity4 && entityB is BulletEntity bulletEntity8) {
                HandleExit_Bullet_Field(bulletEntity8, fieldEntity4);
                return;
            }

            TDLog.Warning($"未处理的碰撞事件<Trigger - Exit>:\n{entityA.IDCom}\n{entityB.IDCom}");
        }

        // Skill - Role
        void HandleEnter_Skill_Role(SkillEntity skill, RoleEntity role, in EntityCollisionEvent evModel) {
            if (!skill.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            var entityColliderModelA = evModel.entityColliderModelA;
            var entityColliderModelB = evModel.entityColliderModelB;
            var fatherA = entityColliderModelA.Father;
            var skillColliderPos = fatherA.IsTheSameAs(skill.IDCom.Father) ? entityColliderModelA.transform.position : entityColliderModelB.transform.position;

            var rootDomain = worldContext.RootDomain;
            _ = rootDomain.TryGetEntityObj(skill.IDCom.Father, out var fatherEntity);
            var casterRole = fatherEntity as RoleEntity;
            var casterPos = casterRole.LogicRootPos;
            var rolePos = role.LogicRootPos;
            var beHitDir = rolePos - casterPos;
            beHitDir.Normalize();

            // 角色 受击
            var roleDomain = rootDomain.RoleDomain;
            roleDomain.HandleBeHit(skill.CurFrame, beHitDir, role, skill.IDCom.ToArgs(), collisionTriggerModel);

            // 技能 打击
            var skillDomain = rootDomain.SkillDomain;
            skillDomain.HandleHit(skill, skillColliderPos, collisionTriggerModel);
        }
     
        void HandleExit_Skill_Role(SkillEntity skill, RoleEntity role) {
        }

        // Bullet - Role
        void HandleTriggerEnter_Bullet_Role(BulletEntity bullet, RoleEntity role) {
            if (!bullet.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            var rolePos = role.LogicRootPos;
            var beHitDir = rolePos - bullet.LogicPos;
            beHitDir.Normalize();

            // 角色 受击
            var rootDomain = worldContext.RootDomain;
            var roleDomain = rootDomain.RoleDomain;
            var hitFrame = bullet.FSMCom.ActivatedModel.curFrame;
            roleDomain.HandleBeHit(hitFrame, beHitDir, role, bullet.IDCom.ToArgs(), collisionTriggerModel);

            // 子弹 打击
            var bulletDomain = rootDomain.BulletDomain;
            bulletDomain.HandleHit(bullet);
        }
     
        void HandleExit_Bullet_Role(BulletEntity bullet, RoleEntity role) {
        }

        // Bullet - Skill
        void HandleEnter_Bullet_Skill(BulletEntity bullet, SkillEntity skill) {
            if (!bullet.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            var rootDomain = worldContext.RootDomain;
            var skillDomain = rootDomain.SkillDomain;
            skillDomain.HandleBeHit(skill, collisionTriggerModel, bullet.FSMCom.ActivatedModel.curFrame);
        }
     
        void HandleExit_Bullet_Skill(BulletEntity bullet, SkillEntity skill) {
        }

        // Bullet - Field
        void HandleExit_Bullet_Field(BulletEntity bullet, FieldEntity field) {
        }

        // Bullet - Bullet
        void HandleEnter_BulletNBullet(BulletEntity bullet1, BulletEntity bullet2) {
            var rootDomain = worldContext.RootDomain;
            var bulletDomain = rootDomain.BulletDomain;
            bulletDomain.HandleHit(bullet1);
            bulletDomain.HandleHit(bullet2);
            bulletDomain.HandleBeHit(bullet1);
            bulletDomain.HandleBeHit(bullet2);
        }
       
        void HandleExit_Bullet_Bullet(BulletEntity bullet1, BulletEntity bullet2) {
        }

        // Bullet - Field
        void HandleEnter_Bullet_Field(BulletEntity bullet, FieldEntity field) {
            var rootDomain = worldContext.RootDomain;
            var bulletFSMDomain = rootDomain.BulletFSMDomain;
            bulletFSMDomain.Enter_Dying(bullet);
        }

        // Role - Field
        void HandleEnter_Role_Field(RoleEntity role, FieldEntity field, in EntityCollisionEvent evModel) {
            var entityA = evModel.entityColliderModelA.Father;
            var entityB = evModel.entityColliderModelB.Father;
            var isRoleA = entityA.IsTheSameAs(role.IDCom.Father);
            var normal = isRoleA ? evModel.normalB : evModel.normalA;
            // 第一次接触地面
            var moveCom = role.MoveCom;
            var rb = moveCom.RB;
            var velo = rb.velocity;
            velo.y = 0;
            moveCom.SetVelocity(velo);
            role.groundCount++;
        }

        void HandleStay_Role_Field(RoleEntity role, FieldEntity field, in EntityCollisionEvent evModel) {
            var entityA = evModel.entityColliderModelA.Father;
            var entityB = evModel.entityColliderModelB.Father;
            var isRoleA = entityA.IsTheSameAs(role.IDCom.Father);
            var normal = isRoleA ? evModel.normalA : -evModel.normalA;
            role.groundCount++;
        }

        void HandleExit_Role_Field(RoleEntity role, FieldEntity field, in EntityCollisionEvent evModel) {
            var entityA = evModel.entityColliderModelA.Father;
            var entityB = evModel.entityColliderModelB.Father;
            var isRoleA = entityA.IsTheSameAs(role.IDCom.Father);
            var normal = isRoleA ? evModel.normalB : evModel.normalA;
            if (normal.y > 0.01f) {
                var fsmCom = role.FSMCom;
                fsmCom.RemovePositionStatus_OnGround();
            }
        }

        // Role - Role
        void HandleEnter_Role_Role(RoleEntity role1, RoleEntity role2) {
        }
     
        void HandleExit_Role_Role(RoleEntity role1, RoleEntity role2) {
        }

        // Skill - Skill
        void HandleExit_Skill_Skill(SkillEntity skill1, SkillEntity skill2) {
        }

    }

}
using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldPhxDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldPhxDomain() {
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = worldDomain;
        }

        public void Tick(float dt) {
            Physics2D.Simulate(dt);

            var collisionEventRepo = worldContext.CollisionEventRepo;

            while (collisionEventRepo.TryPick_TriggerEnter(out var ev)) HandleEnter(ev);
            while (collisionEventRepo.TryPick_CollisionEnter(out var ev)) HandleEnter(ev);

            while (collisionEventRepo.TryPick_TriggerExit(out var ev)) HandleExit(ev);
            while (collisionEventRepo.TryPick_CollisionExit(out var ev)) HandleExit(ev);

            while (collisionEventRepo.TryPick_TriggerStay(out var ev)) HandleStay(ev);
            while (collisionEventRepo.TryPick_CollisionStay(out var ev)) HandleStay(ev);

            var roleRepo = worldContext.RoleRepo;
            roleRepo.Foreach_All((role) => {
                var fsmCom = role.FSMCom;
                if (role.IsGround) {
                    if (!fsmCom.PositionStatus.Contains(RolePositionStatus.OnGround)) fsmCom.AddPositionStatus_OnGround();
                } else {
                    if (fsmCom.PositionStatus.Contains(RolePositionStatus.OnGround)) fsmCom.RemovePositionStatus_OnGround();
                }
                if (role.IDCom.ControlType == ControlType.Player) {
                    TDLog.Log($"groundCount {role.groundCount}\n{role.IDCom}\n{role.FSMCom.PositionStatus.GetString()}");
                }
                role.groundCount = 0;
            });

        }

        #region [碰撞事件处理]

        #region [Enter]

        void HandleEnter(in CollisionEventModel evModel) {
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

            TDLog.Error($"未处理的碰撞事件<Enter>:\n{entityA.IDCom}\n{entityB.IDCom}");
        }

        void HandleEnter_Skill_Role(SkillEntity skill, RoleEntity role, in CollisionEventModel evModel) {
            if (!skill.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            var entityColliderModelA = evModel.entityColliderModelA;
            var entityColliderModelB = evModel.entityColliderModelB;
            var fatherA = entityColliderModelA.Father;
            var skillColliderPos = fatherA.IsTheSameAs(skill.IDCom.Father) ? entityColliderModelA.transform.position : entityColliderModelB.transform.position;

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
            skillDomain.HandleHit(skill, skillColliderPos);
        }

        void HandleTriggerEnter_Bullet_Role(BulletEntity bullet, RoleEntity role) {
            if (!bullet.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            var rolePos = role.LogicRootPos;
            var beHitDir = rolePos - bullet.LogicPos;
            beHitDir.Normalize();

            // 角色 受击
            var roleDomain = rootDomain.RoleDomain;
            var hitFrame = bullet.FSMCom.ActivatedModel.curFrame;
            roleDomain.HandleBeHit(hitFrame, beHitDir, role, bullet.IDCom.ToArgs(), collisionTriggerModel);

            // 子弹 打击
            var bulletDomain = rootDomain.BulletDomain;
            bulletDomain.HandleHit(bullet);
        }

        void HandleEnter_Bullet_Skill(BulletEntity bullet, SkillEntity skill) {
            if (!bullet.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            var skillDomain = rootDomain.SkillDomain;
            skillDomain.HandleBeHit(skill, collisionTriggerModel, bullet.FSMCom.ActivatedModel.curFrame);
        }

        void HandleEnter_Role_Role(RoleEntity role1, RoleEntity role2) {
            // TDLog.Log($"碰撞事件<Trigger - Enter>:\n{role1.IDCom}\n{role2.IDCom}");
        }

        void HandleEnter_BulletNBullet(BulletEntity bullet1, BulletEntity bullet2) {
            var bulletDomain = rootDomain.BulletDomain;
            bulletDomain.HandleHit(bullet1);
            bulletDomain.HandleHit(bullet2);
            bulletDomain.HandleBeHit(bullet1);
            bulletDomain.HandleBeHit(bullet2);
        }

        void HandleEnter_Role_Field(RoleEntity role, FieldEntity field, in CollisionEventModel evModel) {
            var entityA = evModel.entityColliderModelA.Father;
            var entityB = evModel.entityColliderModelB.Father;
            var isRoleA = entityA.IsTheSameAs(role.IDCom.Father);
            var normal = isRoleA ? evModel.normalA : -evModel.normalA;
            var isStand = normal.y > 0.01f;

            if (isStand) {
                // 第一次接触地面
                var moveCom = role.MoveCom;
                var rb = moveCom.RB;
                var velo = rb.velocity;
                velo.y = 0;
                moveCom.SetVelocity(velo);

                role.groundCount++;
            }
        }

        #endregion

        #region [Exit]

        void HandleExit(in CollisionEventModel evModel) {
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

            TDLog.Error($"未处理的碰撞事件<Trigger - Exit>:\n{entityA.IDCom}\n{entityB.IDCom}");
        }

        void HandleExit_Bullet_Bullet(BulletEntity bullet1, BulletEntity bullet2) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{bullet1.IDCom}\n{bullet2.IDCom}");
        }

        void HandleExit_Skill_Skill(SkillEntity skill1, SkillEntity skill2) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{skill1.IDCom}\n{skill2.IDCom}");
        }

        void HandleExit_Skill_Role(SkillEntity skill, RoleEntity role) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{skill.IDCom}\n{role.IDCom}");
        }

        void HandleExit_Role_Role(RoleEntity role1, RoleEntity role2) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{role1.IDCom}\n{role2.IDCom}");
        }

        void HandleExit_Bullet_Role(BulletEntity bullet, RoleEntity role) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{bullet.IDCom}\n{role.IDCom}");
        }

        void HandleExit_Bullet_Skill(BulletEntity bullet, SkillEntity skill) {
            TDLog.Log($"碰撞事件<Trigger - Exit>:\n{bullet.IDCom}\n{skill.IDCom}");
        }

        void HandleExit_Role_Field(RoleEntity role, FieldEntity field, in CollisionEventModel evModel) {
            var entityA = evModel.entityColliderModelA.Father;
            var entityB = evModel.entityColliderModelB.Father;
            var isRoleA = entityA.IsTheSameAs(role.IDCom.Father);
            var normal = isRoleA ? evModel.normalA : -evModel.normalA;
            var isStand = normal.y > 0.01f;

            if (isStand) {
                var fsmCom = role.FSMCom;
                fsmCom.RemovePositionStatus_OnGround();
            }
        }

        #endregion

        #region [Stay]

        void HandleStay(in CollisionEventModel evModel) {
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

        void HandleStay_Role_Field(RoleEntity role, FieldEntity field, in CollisionEventModel evModel) {
            var entityA = evModel.entityColliderModelA.Father;
            var entityB = evModel.entityColliderModelB.Father;
            var isRoleA = entityA.IsTheSameAs(role.IDCom.Father);
            var normal = isRoleA ? evModel.normalA : -evModel.normalA;
            var isStand = normal.y > 0.01f;

            if (isStand) role.groundCount++;
        }

        #endregion

        #endregion

    }

}
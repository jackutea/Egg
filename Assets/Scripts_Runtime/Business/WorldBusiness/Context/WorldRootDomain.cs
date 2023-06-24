using UnityEngine;
using TiedanSouls.Generic;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Domain;
using TiedanSouls.Client.Entities;
using System;

namespace TiedanSouls.Client.Facades {

    public class WorldRootDomain {

        #region [实体 Domain]

        public WorldFSMDomain WorldFSMDomain { get; private set; }
        public WorldFieldDomain FieldDomain { get; private set; }
        public WorldFieldFSMDomain FieldFSMDomain { get; private set; }
        public WorldRoleDomain RoleDomain { get; private set; }
        public WorldRoleFSMDomain RoleFSMDomain { get; private set; }
        public WorldSkillDomain SkillDomain { get; private set; }
        public WorldProjectileDomain ProjectileDomain { get; private set; }
        public WorldProjectileFSMDomain ProjectileFSMDomain { get; private set; }
        public WorldBulletDomain BulletDomain { get; private set; }
        public WorldBulletFSMDomain BulletFSMDomain { get; private set; }
        public WorldBuffDomain BuffDomain { get; private set; }

        #endregion

        #region [杂项 Domain]

        public WorldPhxDomain PhysicalDomain { get; private set; }
        public WorldEffectorDomain EffectorDomain { get; private set; }

        #endregion

        #region [表现层 Domain]

        public WorldRendererDomain WorldRendererDomain { get; private set; }

        #endregion

        #region [上下文]

        public WorldContext worldContext { get; private set; }

        #endregion

        public WorldRootDomain() {
            WorldFSMDomain = new WorldFSMDomain();
            FieldDomain = new WorldFieldDomain();
            FieldFSMDomain = new WorldFieldFSMDomain();
            RoleDomain = new WorldRoleDomain();
            RoleFSMDomain = new WorldRoleFSMDomain();
            SkillDomain = new WorldSkillDomain();
            ProjectileDomain = new WorldProjectileDomain();
            ProjectileFSMDomain = new WorldProjectileFSMDomain();
            BulletDomain = new WorldBulletDomain();
            BulletFSMDomain = new WorldBulletFSMDomain();
            BuffDomain = new WorldBuffDomain();

            PhysicalDomain = new WorldPhxDomain();
            EffectorDomain = new WorldEffectorDomain();

            WorldRendererDomain = new WorldRendererDomain();
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            worldContext.Inject(this);

            this.WorldFSMDomain.Inject(infraContext, worldContext);
            this.FieldDomain.Inject(infraContext, worldContext);
            this.FieldFSMDomain.Inject(infraContext, worldContext);
            this.RoleFSMDomain.Inject(infraContext, worldContext);
            this.RoleDomain.Inject(infraContext, worldContext);
            this.SkillDomain.Inject(infraContext, worldContext);
            this.ProjectileDomain.Inject(infraContext, worldContext);
            this.ProjectileFSMDomain.Inject(infraContext, worldContext);
            this.BulletDomain.Inject(infraContext, worldContext);
            this.BulletFSMDomain.Inject(infraContext, worldContext);
            this.BuffDomain.Inject(infraContext, worldContext);

            this.PhysicalDomain.Inject(infraContext, worldContext);
            this.EffectorDomain.Inject(infraContext, worldContext);

            this.WorldRendererDomain.Inject(infraContext, worldContext);

            this.worldContext = worldContext;
        }

        #region [生成]

        /// <summary>
        /// 实体生成控制模型 --> 生成一个实体
        /// </summary>
        public void SpawnBy_EntitySpawnCtrlModel(in FieldSpawnEntityCtrlModel spawnCtrlModel, int fromFieldTypeID) {
            SpawnBy_EntitySpawnModel(spawnCtrlModel.entitySpawnModel, fromFieldTypeID);
        }

        /// <summary>
        /// 实体生成模型 --> 生成一个实体
        /// </summary>
        public void SpawnBy_EntitySpawnModel(in EntitySpawnModel spawnModel, int fromFieldTypeID) {
            var spawnEntityType = spawnModel.entityType;

            if (spawnEntityType == EntityType.Role) {
                _ = RoleDomain.TrySpawnRole(fromFieldTypeID, spawnModel, out var role);
                role.name = $"角色(生成)_{role.IDCom}";
            } else {
                TDLog.Error($"未知的实体类型 {spawnEntityType}");
            }
        }

        #endregion

        /// <summary>
        /// 从实体ID参数中获取相关角色实体
        /// </summary>
        /// <returns></returns> 
        public bool TryGetRoleFromIDArgs(in EntityIDComponent idArgs, out RoleEntity role) {
            role = null;
            return TryGetRoleRecursively(idArgs, ref role);
        }

        /// <summary>
        /// 递归从实体ID参数中获取相关角色实体
        /// </summary> 
        /// <returns></returns>
        bool TryGetRoleRecursively(in EntityIDComponent idArgs, ref RoleEntity role) {
            if (!TryGetEntityObj(idArgs, out var entity)) return false;

            if (entity is RoleEntity roleEntity) {
                role = roleEntity;
                return true;
            }

            if (entity is SkillEntity skillEntity) {
                var idCom = skillEntity.IDCom;
                var father = idCom.Father;
                return TryGetRoleRecursively(father, ref role);
            }

            if (entity is BulletEntity bulletEntity) {
                var idCom = bulletEntity.IDCom;
                var father = idCom.Father;
                return TryGetRoleRecursively(father, ref role);
            }

            if (entity is ProjectileEntity projectileEntity) {
                var idCom = projectileEntity.IDCom;
                var father = idCom.Father;
                return TryGetRoleRecursively(father, ref role);
            }

            TDLog.Error($"未知的实体类型\n{idArgs}");
            return false;
        }

        public bool TryGetEntityObj(in EntityIDComponent idArgs, out IEntity entity) {
            entity = null;

            var entityType = idArgs.EntityType;
            var entityID = idArgs.EntityID;

            if (entityType == EntityType.Role) {
                var roleRepo = worldContext.RoleRepo;
                if (!roleRepo.TryGet_FromAll(entityID, out var role)) return false;
                entity = role;
                return true;
            }

            if (entityType == EntityType.Skill) {
                var skillRepo = worldContext.SkillRepo;
                if (!skillRepo.TryGet(entityID, out var skill)) return false;
                entity = skill;
                return true;
            }

            if (entityType == EntityType.Bullet) {
                var bulletRepo = worldContext.BulletRepo;
                if (!bulletRepo.TryGet(entityID, out var bullet)) return false;
                entity = bullet;
                return true;
            }

            if (entityType == EntityType.Projectile) {
                var projectileRepo = worldContext.ProjectileRepo;
                if (!projectileRepo.TryGet(entityID, out var projectile)) return false;
                entity = projectile;
                return true;
            }

            if (entityType == EntityType.Field) {
                var fieldRepo = worldContext.FieldRepo;
                if (!fieldRepo.TryGet(entityID, out var field)) return false;
                entity = field;
                return true;
            }

            TDLog.Error($"尚未处理的实体!\n{idArgs}");
            return false;
        }

        #region [实体追踪 目标设置]

        /// <summary>
        /// 根据 实体追踪模型 设置 第一个满足条件的实体目标
        /// </summary>
        public void TrySetEntityTrackTarget(ref EntityTrackModel entityTrackModel, in EntityIDComponent self) {
            var stateEntity = this.worldContext.StateEntity;
            var curFieldTypeID = stateEntity.CurFieldTypeID;

            var entityTrackSelectorModel = entityTrackModel.entityTrackSelectorModel;
            var trackEntityType = entityTrackSelectorModel.entityType;
            var attributeSelectorModel = entityTrackSelectorModel.attributeSelectorModel;

            if (trackEntityType == EntityType.Role) {
                var roleRepo = this.worldContext.RoleRepo;
                if (roleRepo.TryGet_TrackEntity(
                    curFieldTypeID,
                    entityTrackModel.relativeTrackTargetGroupType,
                    self,
                    attributeSelectorModel,
                    out var role
                )) {
                    entityTrackModel.target = role.IDCom;
                }

                return;
            }

            TDLog.Error($"EntityTrack 未处理的实体类型 {trackEntityType}");
            return;
        }

        #endregion

    }

}
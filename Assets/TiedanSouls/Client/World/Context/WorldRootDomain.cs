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
        public WorldItemDomain ItemDomain { get; private set; }
        public WorldProjectileDomain ProjectileDomain { get; private set; }
        public WorldProjectileFSMDomain ProjectileFSMDomain { get; private set; }
        public WorldBulletDomain BulletDomain { get; private set; }
        public WorldBulletFSMDomain BulletFSMDomain { get; private set; }

        #endregion

        #region [杂项 Domain]

        public WorldPhysicsDomain PhysicsDomain { get; private set; }
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
            ItemDomain = new WorldItemDomain();
            ProjectileDomain = new WorldProjectileDomain();
            ProjectileFSMDomain = new WorldProjectileFSMDomain();
            BulletDomain = new WorldBulletDomain();
            BulletFSMDomain = new WorldBulletFSMDomain();

            PhysicsDomain = new WorldPhysicsDomain();
            EffectorDomain = new WorldEffectorDomain();

            WorldRendererDomain = new WorldRendererDomain();
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            worldContext.Inject(this);

            this.WorldFSMDomain.Inject(infraContext, worldContext, this);
            this.FieldDomain.Inject(infraContext, worldContext);
            this.FieldFSMDomain.Inject(infraContext, worldContext);
            this.RoleFSMDomain.Inject(infraContext, worldContext, this);
            this.RoleDomain.Inject(infraContext, worldContext);
            this.SkillDomain.Inject(infraContext, worldContext, this);
            this.ItemDomain.Inject(infraContext, worldContext);
            this.ProjectileDomain.Inject(infraContext, worldContext, this);
            this.ProjectileFSMDomain.Inject(infraContext, worldContext, this);
            this.BulletDomain.Inject(infraContext, worldContext, this);
            this.BulletFSMDomain.Inject(infraContext, worldContext, this);

            this.PhysicsDomain.Inject(infraContext, worldContext, this);
            this.EffectorDomain.Inject(infraContext, worldContext);

            this.WorldRendererDomain.Inject(infraContext, worldContext, this);

            this.worldContext = worldContext;
        }

        #region [生成]

        /// <summary>
        /// 根据 实体召唤模型 生成多个实体
        /// </summary>
        public void SpawnBy_EntitySummonModelArray(Vector3 summonPos, Quaternion baseRot, in IDArgs summoner, in EntitySummonModel[] entitySummonModel) {
            var len = entitySummonModel.Length;
            for (int i = 0; i < len; i++) {
                SpawnBy_EntitySummonModel(summonPos, baseRot, summoner, entitySummonModel[i]);
            }
        }

        /// <summary>
        /// 根据 实体召唤模型 生成一个实体
        /// </summary>
        public void SpawnBy_EntitySummonModel(Vector3 summonPos, Quaternion baseRot, in IDArgs summoner, in EntitySummonModel entitySummonModel) {
            var entityType = entitySummonModel.entityType;

            if (entityType == EntityType.Role) {
                _ = RoleDomain.TrySummonRole(summonPos, baseRot, summoner, entitySummonModel, out var role);
                role.name = $"角色(召唤)_{role.IDCom}";
            } else if (entityType == EntityType.Projectile) {
                _ = ProjectileDomain.TrySummonProjectile(summonPos, baseRot, summoner, entitySummonModel, out _);
            } else {
                TDLog.Error($"未知的实体类型 {entityType}");
            }
        }

        /// <summary>
        /// 实体生成控制模型 --> 生成一个实体
        /// </summary>
        public void SpawnBy_EntitySpawnCtrlModel(in EntitySpawnCtrlModel spawnCtrlModel, int fromFieldTypeID) {
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

        #region [销毁实体]

        /// <summary>
        /// 根据 实体销毁模型 销毁多个实体
        /// </summary>
        public void DestroyBy_EntityDestroyModelArray(in IDArgs summoner, EntityDestroyModel[] entityDestroyModel) {
            var len = entityDestroyModel.Length;
            for (int i = 0; i < len; i++) {
                DestroyBy_EntityDestroyModel(summoner, entityDestroyModel[i]);
            }
        }

        /// <summary>
        /// 根据 实体销毁模型 销毁一个实体  
        /// </summary>
        public void DestroyBy_EntityDestroyModel(in IDArgs summoner, in EntityDestroyModel entityDestroyModel) {
            var entityType = entityDestroyModel.entityType;
            if (entityType == EntityType.None) return;

            var relativeTargetGroupType = entityDestroyModel.relativeTargetGroupType;
            var isEnabled_attributeSelector = entityDestroyModel.isEnabled_attributeSelector;
            var attributeSelectorModel = entityDestroyModel.attributeSelectorModel;
            var curFieldTypeID = worldContext.StateEntity.CurFieldTypeID;
            var roleFSMDomain = worldContext.RootDomain.RoleFSMDomain;

            if (entityType == EntityType.Role) {
                var roleRepo = worldContext.RoleRepo;
                if (isEnabled_attributeSelector) {
                    roleRepo.Foreach_AttributeSelector(
                        curFieldTypeID,
                        relativeTargetGroupType,
                        summoner,
                        attributeSelectorModel,
                        roleFSMDomain.Role_EnterDying
                    );
                }
            } else {
                TDLog.Error($"未知的实体类型 {entityType}");
            }
        }

        #endregion

        #region [碰撞器]

        /// <summary>
        /// 设置 碰撞触发器模型 的父级(为了在碰撞事件触发时找到父级实体)
        /// </summary>
        public void SetFather_CollisionTriggerModelArray(CollisionTriggerModel[] collisionTriggerModelArray, in IDArgs father) {
            var len = collisionTriggerModelArray?.Length;
            for (int i = 0; i < len; i++) {
                var triggerModel = collisionTriggerModelArray[i];
                SetFather_CollisionTriggerModel(triggerModel, father);
            }
        }

        public void SetFather_CollisionTriggerModel(in CollisionTriggerModel triggerModel, in IDArgs father) {
            var array = triggerModel.colliderModelArray;
            SetFather_ColliderModel(array, father);
        }

        public void SetFather_ColliderModel(ColliderModel[] colliderModelArray, in IDArgs father) {
            var len = colliderModelArray.Length;
            for (int i = 0; i < len; i++) {
                var colliderModel = colliderModelArray[i];
                colliderModel.SetFather(father);
                colliderModel.onTriggerEnter2D += AddToCollisionEventRepo_TriggerEnter;
                colliderModel.onTriggerExit2D += AddToCollisionEventRepo_TriggerExit;
            }
        }

        void AddToCollisionEventRepo_TriggerEnter(in CollisionEventArgs args) {
            var evRepo = worldContext.CollisionEventRepo;
            evRepo.Add_TriggerEnter(args);
        }

        void AddToCollisionEventRepo_TriggerExit(in CollisionEventArgs args) {
            var evRepo = worldContext.CollisionEventRepo;
            evRepo.Add_TriggerExit(args);
        }

        #endregion

        #region [获取实体信息]

        public bool TryGetEntityObj(in IDArgs idArgs, out IEntity entity) {
            entity = null;

            var entityType = idArgs.entityType;
            var entityID = idArgs.entityID;

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

            TDLog.Error($"尚未处理的实体!\n{idArgs}");
            return false;
        }

        /// <summary>
        /// 获取实体的位置坐标
        /// </summary>
        public bool TryGetEntityPos(IEntity entity, out Vector3 pos) {
            if (entity is RoleEntity role) {
                pos = role.LogicPos;
                return true;
            }

            // TODO 技能的位置坐标
            if (entity is SkillEntity skill) {
                pos = Vector3.zero;
                return true;
            }

            if (entity is BulletEntity bullet) {
                pos = bullet.LogicPos;
                return true;
            }

            pos = Vector3.zero;
            TDLog.Error($"尚未处理的实体!\n{entity}");
            return false;
        }

        #endregion

        #region [实体追踪 目标设置]

        /// <summary>
        /// 根据 实体追踪模型 设置 第一个满足条件的实体目标
        /// </summary>
        public void TrySetEntityTrackTarget(ref EntityTrackModel entityTrackModel, in IDArgs self) {
            var stateEntity = this.worldContext.StateEntity;
            var curFieldTypeID = stateEntity.CurFieldTypeID;

            var entityTrackSelectorModel = entityTrackModel.entityTrackSelectorModel;
            var trackEntityType = entityTrackSelectorModel.entityType;
            var attributeSelectorModel = entityTrackSelectorModel.attributeSelectorModel;

            if (trackEntityType == EntityType.Role) {
                var roleRepo = this.worldContext.RoleRepo;
                if (roleRepo.TryGet_EntityTrackOne(
                    curFieldTypeID,
                    entityTrackModel.relativeTrackTargetGroupType,
                    self,
                    attributeSelectorModel,
                    out var role
                )) {
                    entityTrackModel.target = role.IDCom.ToArgs();
                }

                return;
            }


            TDLog.Error($"EntityTrack 未处理的实体类型 {trackEntityType}");
            return;
        }

        #endregion

    }

}
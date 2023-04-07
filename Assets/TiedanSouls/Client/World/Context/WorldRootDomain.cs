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
            ItemDomain = new WorldItemDomain();
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
            this.BuffDomain.Inject(infraContext, worldContext, this);

            this.PhysicalDomain.Inject(infraContext, worldContext, this);
            this.EffectorDomain.Inject(infraContext, worldContext);

            this.WorldRendererDomain.Inject(infraContext, worldContext, this);

            this.worldContext = worldContext;
        }

        #region [生成]

        /// <summary>
        /// 根据 实体召唤模型 生成多个实体
        /// </summary>
        public void SpawnBy_EntitySummonModelArray(Vector3 summonPos, Quaternion baseRot, in EntityIDArgs summoner, in EntitySummonModel[] entitySummonModel) {
            var len = entitySummonModel.Length;
            for (int i = 0; i < len; i++) {
                SpawnBy_EntitySummonModel(summonPos, baseRot, summoner, entitySummonModel[i]);
            }
        }

        /// <summary>
        /// 根据 实体召唤模型 生成一个实体
        /// </summary>
        public void SpawnBy_EntitySummonModel(Vector3 summonPos, Quaternion baseRot, in EntityIDArgs summoner, in EntitySummonModel entitySummonModel) {
            var entityType = entitySummonModel.entityType;

            if (entityType == EntityType.Role) {
                _ = RoleDomain.TrySummon(summonPos, baseRot, summoner, entitySummonModel, out var role);
                role.name = $"角色(召唤)_{role.IDCom}";
            } else if (entityType == EntityType.Projectile) {
                _ = ProjectileDomain.TrySummon(summonPos, baseRot, summoner, entitySummonModel, out var projectile);
            } else if (entityType == EntityType.Buff) {
                _ = BuffDomain.TrySummon(summoner, entitySummonModel, out var buff);
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
        public void DestroyBy_EntityDestroyModelArray(in EntityIDArgs summoner, EntityDestroyModel[] entityDestroyModel) {
            var len = entityDestroyModel.Length;
            for (int i = 0; i < len; i++) {
                DestroyBy_EntityDestroyModel(summoner, entityDestroyModel[i]);
            }
        }

        /// <summary>
        /// 根据 实体销毁模型 销毁一个实体  
        /// </summary>
        public void DestroyBy_EntityDestroyModel(in EntityIDArgs summoner, in EntityDestroyModel entityDestroyModel) {
            var entityType = entityDestroyModel.entityType;
            if (entityType == EntityType.None) return;

            var hitTargetGroupType = entityDestroyModel.hitTargetGroupType;
            var isEnabled_attributeSelector = entityDestroyModel.isEnabled_attributeSelector;
            var attributeSelectorModel = entityDestroyModel.attributeSelectorModel;
            var curFieldTypeID = worldContext.StateEntity.CurFieldTypeID;
            var roleFSMDomain = worldContext.RootDomain.RoleFSMDomain;

            if (entityType == EntityType.Role) {
                var roleRepo = worldContext.RoleRepo;
                if (isEnabled_attributeSelector) {
                    roleRepo.Foreach_AttributeSelector(
                        curFieldTypeID,
                        hitTargetGroupType,
                        summoner,
                        attributeSelectorModel,
                        roleFSMDomain.Enter_Dying
                    );
                }
            } else {
                TDLog.Error($"未知的实体类型 {entityType}");
            }
        }

        #endregion

        #region [碰撞器]

        public void SetEntityColliderTriggerModelFathers(EntityColliderTriggerModel[] collisionTriggerModelArray, in EntityIDArgs father) {
            var len = collisionTriggerModelArray?.Length;
            for (int i = 0; i < len; i++) {
                var triggerModel = collisionTriggerModelArray[i];
                SetEntityColliderTriggerModelFather(triggerModel, father);
            }
        }

        public void SetEntityColliderTriggerModelFather(in EntityColliderTriggerModel triggerModel, in EntityIDArgs father) {
            var array = triggerModel.entityColliderArray;
            SetEntityColliderFathers(array, father);
        }

        public void SetEntityColliderFathers(EntityCollider[] colliderModelArray, in EntityIDArgs father) {
            var len = colliderModelArray.Length;
            for (int i = 0; i < len; i++) {
                SetEntityColliderFather(colliderModelArray[i], father);
            }
        }

        /// <summary>
        /// 设置 碰撞器模型 的父级(为了在碰撞事件触发时找到父级实体)
        /// </summary>
        public void SetEntityColliderFather(EntityCollider colliderModel, in EntityIDArgs father) {
            colliderModel.SetFather(father);
            colliderModel.onTriggerEnter2D = AddToCollisionEventRepo_TriggerEnter;
            colliderModel.onTriggerStay2D = AddToCollisionEventRepo_TriggerStay;
            colliderModel.onTriggerExit2D = AddToCollisionEventRepo_TriggerExit;
            colliderModel.onCollisionEnter2D = AddToCollisionEventRepo_CollisionEnter;
            colliderModel.onCollisionStay2D = AddToCollisionEventRepo_CollisionStay;
            colliderModel.onCollisionExit2D = AddToCollisionEventRepo_CollisionExit;
        }

        void AddToCollisionEventRepo_TriggerEnter(EntityCollider entityColliderA, EntityCollider entityColliderB, Vector3 normalB) {
            if (!IsValidCollisionEvent(entityColliderA, entityColliderB)) {
                return;
            }
            var evRepo = worldContext.CollisionEventRepo;
            evRepo.Add_TriggerEnter(entityColliderA, entityColliderB);
        }

        void AddToCollisionEventRepo_TriggerStay(EntityCollider entityColliderA, EntityCollider entityColliderB, Vector3 normalB) {
            if (!IsValidCollisionEvent(entityColliderA, entityColliderB)) {
                return;
            }
            var evRepo = worldContext.CollisionEventRepo;
            evRepo.Add_TriggerStay(entityColliderA, entityColliderB);
        }

        void AddToCollisionEventRepo_TriggerExit(EntityCollider entityColliderA, EntityCollider entityColliderB, Vector3 normalB) {
            if (!IsValidCollisionEvent(entityColliderA, entityColliderB)) {
                return;
            }
            var evRepo = worldContext.CollisionEventRepo;
            evRepo.Add_TriggerExit(entityColliderA, entityColliderB);
        }

        void AddToCollisionEventRepo_CollisionEnter(EntityCollider entityColliderA, EntityCollider entityColliderB, Vector3 normalB) {
            if (!IsValidCollisionEvent(entityColliderA, entityColliderB)) {
                return;
            }
            var evRepo = worldContext.CollisionEventRepo;
            evRepo.Add_CollisionEnter(entityColliderA, entityColliderB, normalB);
        }

        void AddToCollisionEventRepo_CollisionStay(EntityCollider entityColliderA, EntityCollider entityColliderB, Vector3 normalB) {
            if (!IsValidCollisionEvent(entityColliderA, entityColliderB)) {
                return;
            }
            var evRepo = worldContext.CollisionEventRepo;
            evRepo.Add_CollisionStay(entityColliderA, entityColliderB, normalB);
        }

        void AddToCollisionEventRepo_CollisionExit(EntityCollider entityColliderA, EntityCollider entityColliderB, Vector3 normalB) {
            if (!IsValidCollisionEvent(entityColliderA, entityColliderB)) {
                return;
            }
            var evRepo = worldContext.CollisionEventRepo;
            evRepo.Add_CollisionExit(entityColliderA, entityColliderB);
        }

        #endregion

        #region [获取实体信息]

        public bool TryFindRoleFather(in EntityIDArgs idArgs, ref RoleEntity role) {
            if (!TryGetEntityObj(idArgs, out var entity)) return false;

            if (entity is RoleEntity roleEntity) {
                role = roleEntity;
                return true;
            }

            if (entity is SkillEntity skillEntity) {
                var idCom = skillEntity.IDCom;
                var father = idCom.Father;
                return TryFindRoleFather(father, ref role);
            }

            if (entity is BulletEntity bulletEntity) {
                var idCom = bulletEntity.IDCom;
                var father = idCom.Father;
                return TryFindRoleFather(father, ref role);
            }

            if (entity is ProjectileEntity projectileEntity) {
                var idCom = projectileEntity.IDCom;
                var father = idCom.Father;
                return TryFindRoleFather(father, ref role);
            }

            TDLog.Error($"未知的实体类型\n{idArgs}");
            return false;
        }

        public bool TryGetEntityObj(in EntityIDArgs idArgs, out IEntity entity) {
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

        /// <summary>
        /// 获取实体的位置坐标
        /// </summary>
        public bool TryGetEntityPos(IEntity entity, out Vector3 pos) {
            if (entity is RoleEntity role) {
                pos = role.LogicRootPos;
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
        public void TrySetEntityTrackTarget(ref EntityTrackModel entityTrackModel, in EntityIDArgs self) {
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
                    entityTrackModel.target = role.IDCom.ToArgs();
                }

                return;
            }


            TDLog.Error($"EntityTrack 未处理的实体类型 {trackEntityType}");
            return;
        }

        #endregion

        public bool IsValidCollisionEvent(EntityCollider entityColliderA, EntityCollider entityColliderB) {
            var fatherA = entityColliderA.Father;
            var fatherB = entityColliderB.Father;
            var hitTargetGroupTypeA = entityColliderA.HitTargetGroupType;
            var hitTargetGroupTypeB = entityColliderB.HitTargetGroupType;

            if (!IsInRightTargetGroup(hitTargetGroupTypeA, fatherA, fatherB)
            && !IsInRightTargetGroup(hitTargetGroupTypeB, fatherB, fatherA)) {
                return false;
            }

            return true;
        }

        bool IsInRightTargetGroup(TargetGroupType hitTargetGroupType, in EntityIDArgs self, in EntityIDArgs other) {
            if (self.entityType == EntityType.Field || other.entityType == EntityType.Field) {
                // TODO: 移除这里对Field的特殊处理
                return true;
            }

            var selfAllyType = self.allyType;
            var otherAllyType = other.allyType;
            bool isSelf = self.IsTheSameAs(other);
            bool isAlly = selfAllyType.IsAlly(otherAllyType);
            bool isEnemy = selfAllyType.IsEnemy(otherAllyType);
            bool isOtherNeutral = otherAllyType == AllyType.Neutral;

            if (hitTargetGroupType == TargetGroupType.None)
                return false;

            if (hitTargetGroupType.Contains(TargetGroupType.Self) && isSelf)
                return true;

            if (hitTargetGroupType.Contains(TargetGroupType.Ally) && isAlly)
                return true;

            if (hitTargetGroupType.Contains(TargetGroupType.Enemy) && isEnemy)
                return true;

            if (hitTargetGroupType.Contains(TargetGroupType.Neutral) && isOtherNeutral)
                return true;

            return false;
        }

    }

}
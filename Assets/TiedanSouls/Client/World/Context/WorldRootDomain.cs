using UnityEngine;
using TiedanSouls.Generic;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Domain;
using TiedanSouls.Client.Entities;
using System;
using System.Collections.Generic;

namespace TiedanSouls.Client.Facades {

    public class WorldRootDomain {

        #region [实体Domain]

        public WorldFSMDomain WorldFSMDomain { get; private set; }

        public WorldFieldDomain FieldDomain { get; private set; }
        public WorldFieldFSMDomain FieldFSMDomain { get; private set; }

        public WorldRoleDomain RoleDomain { get; private set; }
        public WorldRoleFSMDomain RoleFSMDomain { get; private set; }

        public WorldSkillDomain SkillDomain { get; private set; }

        #endregion

        #region [Misc Domain]

        public WorldPhysicsDomain PhysicsDomain { get; private set; }
        public WorldHitDomain HitDomain { get; private set; }
        public WorldEffectorDomain EffectorDomain { get; private set; }

        #endregion

        #region [Renderer Domain]

        public WorldRendererDomain WorldRendererDomain { get; private set; }

        #endregion

        public WorldContext WorldContext { get; private set; }

        public WorldRootDomain() {

            WorldFSMDomain = new WorldFSMDomain();
            FieldDomain = new WorldFieldDomain();
            FieldFSMDomain = new WorldFieldFSMDomain();
            RoleDomain = new WorldRoleDomain();
            RoleFSMDomain = new WorldRoleFSMDomain();
            SkillDomain = new WorldSkillDomain();

            PhysicsDomain = new WorldPhysicsDomain();
            HitDomain = new WorldHitDomain();
            EffectorDomain = new WorldEffectorDomain();

            WorldRendererDomain = new WorldRendererDomain();
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            worldContext.Inject(this);

            this.WorldFSMDomain.Inject(infraContext, worldContext, this);
            this.RoleFSMDomain.Inject(infraContext, worldContext, this);
            this.RoleDomain.Inject(infraContext, worldContext);
            this.SkillDomain.Inject(infraContext, worldContext);
            this.FieldDomain.Inject(infraContext, worldContext);
            this.FieldFSMDomain.Inject(infraContext, worldContext);

            this.PhysicsDomain.Inject(infraContext, worldContext, this);
            this.HitDomain.Inject(infraContext, worldContext, this);
            this.EffectorDomain.Inject(infraContext, worldContext);

            this.WorldRendererDomain.Inject(infraContext, worldContext, this);

            this.WorldContext = worldContext;
        }

        #region [Spawn]

        public void SpawnBy_EntitySummonModel(in EntitySummonModel entitySummonModel, in IDArgs summoner, Vector3 spawnPos) {
            var controlType = entitySummonModel.controlType;
            var entityType = entitySummonModel.entityType;
            var typeID = entitySummonModel.typeID;
            var allyType = summoner.allyType;

            if (entityType == EntityType.Role) {
                var role = RoleDomain.SpawnRole(controlType, typeID, allyType, spawnPos);
            } else {
                TDLog.Error($"未知的实体类型 {entityType}");
            }
        }

        public void SpawnBy_EntitySpawnCtrlModelArray(EntitySpawnCtrlModel[] spawnCtrlModelArray) {
            var spawnCount = spawnCtrlModelArray?.Length;
            for (int i = 0; i < spawnCount; i++) {
                SpawnBy_EntitySpawnCtrlModel(spawnCtrlModelArray[i]);
            }
        }

        public void SpawnBy_EntitySpawnCtrlModel(in EntitySpawnCtrlModel spawnModel) {
            var entityType = spawnModel.entityType;
            var typeID = spawnModel.typeID;
            var roleControlType = spawnModel.controlType;
            var allyType = spawnModel.allyType;
            var controlType = spawnModel.controlType;
            var isBoss = spawnModel.isBoss;
            var spawnPos = spawnModel.pos;

            if (entityType == EntityType.Role) {
                var role = RoleDomain.SpawnRole(controlType, typeID, allyType, spawnPos);
                role.SetIsBoss(isBoss);
                TDLog.Log($"人物: AllyType {allyType} / ControlType {controlType} / TypeID {typeID} / Name {role.IDCom.EntityName} / IsBoss {isBoss} Spawned!");
            } else {
                TDLog.Error($"未知的实体类型 {entityType}");
            }
        }

        #endregion

        #region [Destroy]

        public void DestroyBy_EntityDestroyModel(in EntityDestroyModel entityDestroyModel, in IDArgs summoner) {
            var entityType = entityDestroyModel.entityType;
            if (entityType == EntityType.None) return;

            var targetType = entityDestroyModel.targetType;
            var isEnabled_attributeSelector = entityDestroyModel.isEnabled_attributeSelector;
            var attributeSelectorModel = entityDestroyModel.attributeSelectorModel;
            var curFieldTypeID = WorldContext.StateEntity.CurFieldTypeID;
            var roleDomain = WorldContext.RootDomain.RoleDomain;

            if (entityType == EntityType.Role) {
                var roleRepo = WorldContext.RoleRepo;
                var list = roleRepo.GetRoleList_ByTargetType(curFieldTypeID, targetType, summoner);
                list.ForEach(((role) => {
                    var attributeCom = role.AttributeCom;
                    // 选择器 - 属性
                    if (isEnabled_attributeSelector && !attributeCom.IsMatch(attributeSelectorModel)) return;
                    roleDomain.Role_PrepareToDie(role);
                }));

            } else {
                TDLog.Error($"未知的实体类型 {entityType}");
            }
        }

        #endregion

        #region [CollisionTrigger]

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
            var evRepo = WorldContext.CollisionEventRepo;
            evRepo.Add_TriggerEnter(args);
        }

        void AddToCollisionEventRepo_TriggerExit(in CollisionEventArgs args) {
            var evRepo = WorldContext.CollisionEventRepo;
            evRepo.Add_TriggerExit(args);
        }

        #endregion

        #region [Entity]

        public bool TryGetEntityObj(in IDArgs idArgs, out IEntity entity) {
            entity = null;

            var entityType = idArgs.entityType;
            var entityID = idArgs.entityID;

            if (entityType == EntityType.Role) {
                var roleRepo = WorldContext.RoleRepo;
                if (!roleRepo.TryGet_FromAll(entityID, out var role)) return false;
                entity = role;
                return true;
            }

            if (entityType == EntityType.Skill) {
                var skillRepo = WorldContext.SkillRepo;
                if (!skillRepo.TryGet(entityID, out var skill)) return false;
                entity = skill;
                return true;
            }

            TDLog.Error($"尚未处理的实体!\n{idArgs}");
            return false;
        }

        #endregion

    }
}
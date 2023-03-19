using TiedanSouls.Generic;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Domain;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client.Facades {

    public class WorldRootDomain {

        public WorldFSMDomain GameDomain { get; private set; }

        public WorldFieldDomain FieldDomain { get; private set; }
        public WorldFieldFSMDomain FieldFSMDomain { get; private set; }

        public WorldRoleDomain RoleDomain { get; private set; }
        public WorldRoleFSMDomain RoleFSMDomain { get; private set; }

        public WorldSkillDomain SkillDomain { get; private set; }

        public WorldPhysicsDomain PhysicsDomain { get; private set; }
        public WorldHitDomain HitDomain { get; private set; }

        public WorldRendererDomain WorldRendererDomain { get; private set; }

        public WorldContext WorldContext { get; private set; }

        public WorldRootDomain() {
            GameDomain = new WorldFSMDomain();

            FieldDomain = new WorldFieldDomain();
            FieldFSMDomain = new WorldFieldFSMDomain();

            RoleDomain = new WorldRoleDomain();
            RoleFSMDomain = new WorldRoleFSMDomain();

            SkillDomain = new WorldSkillDomain();

            PhysicsDomain = new WorldPhysicsDomain();

            HitDomain = new WorldHitDomain();

            WorldRendererDomain = new WorldRendererDomain();
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            worldContext.Inject(this);

            this.GameDomain.Inject(infraContext, worldContext, this);

            this.RoleFSMDomain.Inject(infraContext, worldContext, this);
            this.RoleDomain.Inject(infraContext, worldContext);

            this.SkillDomain.Inject(infraContext, worldContext);

            this.FieldDomain.Inject(infraContext, worldContext);
            this.FieldFSMDomain.Inject(infraContext, worldContext);

            this.PhysicsDomain.Inject(infraContext, worldContext, this);
            this.HitDomain.Inject(infraContext, worldContext, this);


            this.WorldRendererDomain.Inject(infraContext, worldContext, this);

            this.WorldContext = worldContext;
        }

        #region [Spawn]

        public void SpawnByModelArray(FieldSpawnModel[] spawnModelArray) {
            var spawnCount = spawnModelArray?.Length;
            for (int i = 0; i < spawnCount; i++) {
                SpawnByModel(spawnModelArray[i]);
            }
        }

        public void SpawnByModel(in FieldSpawnModel spawnModel) {
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
                TDLog.Error("Not Handle Yet!");
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
                if (!roleRepo.TryGet(entityID, out var role)) return false;
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
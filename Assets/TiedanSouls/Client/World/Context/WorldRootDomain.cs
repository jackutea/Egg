using TiedanSouls.Generic;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Domain;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client.Facades {

    public class WorldRootDomain {

        WorldFSMDomain gameDomain;
        public WorldFSMDomain GameDomain => gameDomain;

        WorldFieldDomain fieldDomain;
        public WorldFieldDomain FieldDomain => fieldDomain;

        WorldFieldFSMDomain fieldFSMDomain;
        public WorldFieldFSMDomain FieldFSMDomain => fieldFSMDomain;

        WorldRoleDomain roleDomain;
        public WorldRoleDomain RoleDomain => roleDomain;

        WorldRoleFSMDomain roleFSMDomain;
        public WorldRoleFSMDomain RoleFSMDomain => roleFSMDomain;

        WorldPhysicsDomain phxDomain;
        public WorldPhysicsDomain WorldPhysicsDomain => phxDomain;

        WorldRendererDomain worldRendererDomain;
        public WorldRendererDomain WorldRendererDomain => worldRendererDomain;

        WorldContext worldContext;
        public WorldContext WorldContext => worldContext;

        public WorldRootDomain() {
            gameDomain = new WorldFSMDomain();

            fieldDomain = new WorldFieldDomain();
            fieldFSMDomain = new WorldFieldFSMDomain();

            roleDomain = new WorldRoleDomain();
            roleFSMDomain = new WorldRoleFSMDomain();

            phxDomain = new WorldPhysicsDomain();

            worldRendererDomain = new WorldRendererDomain();
        }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            worldContext.Inject(this);

            GameDomain.Inject(infraContext, worldContext, this);

            RoleFSMDomain.Inject(infraContext, worldContext, this);
            RoleDomain.Inject(infraContext, worldContext);

            FieldDomain.Inject(infraContext, worldContext);
            FieldFSMDomain.Inject(infraContext, worldContext);

            WorldPhysicsDomain.Inject(infraContext, worldContext, this);

            WorldRendererDomain.Inject(infraContext, worldContext, this);

            this.worldContext = worldContext;
        }

        #region [Spawn]

        public void SpawnByModelArray(SpawnModel[] spawnModelArray) {
            var spawnCount = spawnModelArray?.Length;
            for (int i = 0; i < spawnCount; i++) {
                SpawnByModel(spawnModelArray[i]);
            }
        }

        public void SpawnByModel(in SpawnModel spawnModel) {
            var entityType = spawnModel.entityType;
            var typeID = spawnModel.typeID;
            var roleControlType = spawnModel.controlType;
            var allyType = spawnModel.allyType;
            var controlType = spawnModel.controlType;
            var isBoss = spawnModel.isBoss;
            var spawnPos = spawnModel.pos;

            if (entityType == EntityType.Role) {
                var role = roleDomain.SpawnRole(controlType, typeID, allyType, spawnPos);
                role.SetIsBoss(isBoss);
                TDLog.Log($"人物: AllyType {allyType} / ControlType {controlType} / TypeID {typeID} / Name {role.IDCom.EntityName} / IsBoss {isBoss} Spawned!");
            } else {
                TDLog.Error("Not Handle Yet!");
            }
        }

        #endregion

        #region [CollisionTrigger]

        public void SetFather_CollisionTriggerModelArray(CollisionTriggerModel[] collisionTriggerModelArray, in IDArgs father) {
            var len = collisionTriggerModelArray.Length;
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
                colliderModel.onTriggerStay2D += AddToCollisionEventRepo_TriggerStay;
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

        void AddToCollisionEventRepo_TriggerStay(in CollisionEventArgs args) {
            var evRepo = worldContext.CollisionEventRepo;
            evRepo.Add_TriggerStay(args);
        }

        #endregion

    }
}
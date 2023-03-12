using TiedanSouls.Generic;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Domain;

namespace TiedanSouls.World.Facades {

    public class WorldDomain {

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

        public WorldDomain() {
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
        }

        public void SpawnByModelArray(SpawnModel[] spawnModelArray) {
            var spawnCount = spawnModelArray?.Length;
            for (int i = 0; i < spawnCount; i++) {
                SpawnByModel(spawnModelArray[i]);
            }
        }

        public void SpawnByModel(SpawnModel spawnModel) {
            var entityType = spawnModel.entityType;
            var typeID = spawnModel.typeID;
            var roleControlType = spawnModel.controlType;
            var allyType = spawnModel.allyType;
            var ownerRoleSpawnPos = spawnModel.pos;
            if (entityType == EntityType.Role) {
                var role = roleDomain.SpawnRole(roleControlType, typeID, allyType, ownerRoleSpawnPos);
                role.SetIsBoss(spawnModel.isBoss);
                TDLog.Log($"人物: AllyType {allyType} / ControlType {role.ControlType} / TypeID {typeID} / RoleName {role.IDCom.EntityName} IsBoss {role.IsBoss} Spawned!");
            } else {
                TDLog.Error("Not Handle Yet!");
            }


        }

    }
}
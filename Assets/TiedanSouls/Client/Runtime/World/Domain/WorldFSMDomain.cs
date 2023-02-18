using System;
using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;

namespace TiedanSouls.World.Domain {

    public class WorldFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldDomain worldDomain;

        public WorldFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void EnterHall() {
            // Config
            var gameConfigTM = infraContext.TemplateCore.GameConfigTM;
            var firstFieldTypeID = gameConfigTM.hallFieldTypeID;
            var tieDanRoleTypeID = gameConfigTM.tiedanRoleTypeID;
            var hallWeaponTypeIDs = gameConfigTM.hallWeaponTypeIDs;

            var fieldDomain = worldDomain.FieldDomain;
            var field = fieldDomain.SpawnField(firstFieldTypeID);

            // Check
            var fieldType = field.fieldType;
            if (fieldType != FieldType.Hall) {
                TDLog.Error("进入大厅失败! FieldType: {fieldType}");
                return;
            }

            // Hall Character
            TDLog.Log($"大厅人物生成 firstFieldTypeID {firstFieldTypeID}-------------------------------------------- ");
            var spawnModelArray = field.SpawnModelArray;
            var spawnCount = spawnModelArray?.Length;
            var roleDomain = worldDomain.RoleDomain;
            for (int i = 0; i < spawnCount; i++) {
                var spawnModel = spawnModelArray[i];
                var entityType = spawnModel.entityType;
                var typeID = spawnModel.typeID;
                var roleControlType = spawnModel.roleControlType;
                var allyType = spawnModel.allyType;
                var ownerRoleSpawnPos = spawnModel.pos;
                if (entityType == EntityType.Role) {
                    var role = roleDomain.SpawnRole(roleControlType, typeID, allyType, ownerRoleSpawnPos);
                    TDLog.Log($"AllyType: {allyType} / ControlType:{role.ControlType} / TypeID: {typeID} / RoleName: {role.RoleName}");
                } else {
                    TDLog.Error("Not Handle Yet!");
                }
            }

            // Spawn TieDan 
            var owner = roleDomain.SpawnRole(RoleControlType.Player, tieDanRoleTypeID, AllyType.Player, new Vector2(5, 5));

            // TODO: Spawn Weapon For TieDan To Choose 
            var weaponCount = hallWeaponTypeIDs.Length;
            for (int i = 0; i < weaponCount; i++) {
                var id = hallWeaponTypeIDs[i];
                var model = worldContext.WorldFactory.SpawnWeaponModel(id);

            }

            // Set Camera 
            var cameraSetter = infraContext.CameraCore.SetterAPI;
            cameraSetter.Follow_Current(owner.transform, new Vector3(0, 0, -10), EasingType.Immediate, 1f, EasingType.Linear, 1f);
            cameraSetter.Confiner_Set_Current(true, field.transform.position, (Vector2)field.transform.position + field.ConfinerSize);

            // Physics
            Physics2D.IgnoreLayerCollision(LayerCollection.ROLE, LayerCollection.ROLE, true);

            // World State
            var stateEntity = worldContext.StateEntity;
            stateEntity.EnterState_Hall(owner.ID);
        }

        public void ApplyWorldState(float dt) {
            var stateEntity = worldContext.StateEntity;
            var worldStatus = stateEntity.Status;
            if (worldStatus == WorldFSMStatus.Hall) {
                ApplyWorldState_Hall(dt);
            } else if (worldStatus == WorldFSMStatus.BattleField) {
                ApplyWorldState_Battle(dt);
            }
        }

        public void ApplyWorldState_Hall(float dt) {
            ApplyBasicLogic(dt);
        }

        public void ApplyWorldState_Battle(float dt) {
            ApplyBasicLogic(dt);

            var roleRepo = worldContext.RoleRepo;
            var allRole = roleRepo.GetAll();
            foreach (var role in allRole) {
                if (role.gameObject.transform.position.y < 0) {
                    role.DropBeHurt(50, new Vector2(3, 3));
                }
                worldDomain.RoleDomain.TickHUD(role, dt);
            }

            CleanupRole();
        }

        void ApplyBasicLogic(float dt) {
            var roleDomain = worldDomain.RoleDomain;
            var roleFSMDomain = worldDomain.RoleFSMDomain;
            var stateEntity = worldContext.StateEntity;
            var roleRepo = worldContext.RoleRepo;
            var allRole = roleRepo.GetAll();
            foreach (var role in allRole) {
                // Input
                if (role.ID == stateEntity.OwnerRoleID) {
                    roleDomain.RecordOwnerInput(role);
                }

                // AI
                if (role.ControlType == RoleControlType.AI) {
                    role.AIStrategy.Tick(dt);
                }

                // Role FSM
                roleFSMDomain.Tick(role, dt);
            }

            // Physics
            var phxDomain = worldDomain.WorldPhysicsDomain;
            phxDomain.Tick(dt);
        }

        void CleanupRole() {

            var roleRepo = worldContext.RoleRepo;
            var allRole = roleRepo.GetAll();

            Span<int> waitRemove = stackalloc int[roleRepo.Count];
            int count = 0;

            foreach (var role in allRole) {
                if (role.FSMCom.Status == RoleFSMStatus.Dead) {
                    role.TearDown();
                    waitRemove[count] = role.ID;
                    count += 1;
                }
            }

            for (int i = 0; i < count; i += 1) {
                roleRepo.RemoveByID(waitRemove[i]);
            }

        }

    }
}
using System;
using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using System.Collections.Generic;

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

        public void EnterLobby() {
            // Config
            var gameConfigTM = infraContext.TemplateCore.GameConfigTM;

            // Spawn Field
            var firstFieldTypeID = gameConfigTM.lobbyFieldTypeID;
            var fieldDomain = worldDomain.FieldDomain;
            var field = fieldDomain.SpawnField(firstFieldTypeID);

            // Check
            var fieldType = field.FieldType;
            if (fieldType != FieldType.Lobby) {
                TDLog.Error("进入大厅失败! FieldType: {fieldType}");
                return;
            }

            // Lobby Character
            TDLog.Log($"大厅人物生成开始 -------------------------------------------- ");
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
                    TDLog.Log($"人物: AllyType {allyType} / ControlType {role.ControlType} / TypeID {typeID} / RoleName {role.RoleName}");
                } else {
                    TDLog.Error("Not Handle Yet!");
                }
            }
            TDLog.Log($"大厅人物生成结束 -------------------------------------------- ");

            TDLog.Log($"大厅物件生成开始 -------------------------------------------- ");
            var lobbyItemTypeIDs = gameConfigTM.lobbyItemTypeIDs;
            var itemCount = lobbyItemTypeIDs?.Length;
            var itemSpawnPosArray = field.ItemSpawnPosArray;
            for (int i = 0; i < itemCount; i++) {
                if (i >= itemSpawnPosArray.Length) {
                    TDLog.Error("物件生成位置不足!");
                    break;
                }

                var typeID = lobbyItemTypeIDs[i];
                var itemSpawnPos = itemSpawnPosArray[i];
                var itemEntity = worldContext.WorldFactory.SpawnItemEntity(typeID, itemSpawnPos);
                TDLog.Log($"物件: EntityID: {itemEntity.ID} / TypeID {itemEntity.TypeID} / ItemType {itemEntity.ItemType} / TypeIDForPickUp {itemEntity.TypeIDForPickUp}");
            }
            TDLog.Log($"大厅物件生成结束 -------------------------------------------- ");

            // Spawn TieDan 
            var tieDanRoleTypeID = gameConfigTM.tiedanRoleTypeID;
            var owner = roleDomain.SpawnRole(RoleControlType.Player, tieDanRoleTypeID, AllyType.Player, new Vector2(5, 5));

            // Set Camera 
            var cameraSetter = infraContext.CameraCore.SetterAPI;
            cameraSetter.Follow_Current(owner.transform, new Vector3(0, 0, -10), EasingType.Immediate, 1f, EasingType.Linear, 1f);
            cameraSetter.Confiner_Set_Current(true, field.transform.position, (Vector2)field.transform.position + field.ConfinerSize);

            // World State
            var stateEntity = worldContext.StateEntity;
            stateEntity.EnterState_Lobby(owner.ID);
        }

        public void ApplyWorldState(float dt) {
            var stateEntity = worldContext.StateEntity;
            var worldStatus = stateEntity.Status;
            if (worldStatus == WorldFSMStatus.Lobby) {
                ApplyWorldState_Lobby(dt);
            } else if (worldStatus == WorldFSMStatus.BattleField) {
                ApplyWorldState_Battle(dt);
            }
        }

        public void ApplyWorldState_Lobby(float dt) {
            ApplyBasicLogic(dt);
        }

        public void ApplyWorldState_Battle(float dt) {
            ApplyBasicLogic(dt);

            var roleRepo = worldContext.RoleRepo;
            roleRepo.ForeachAll((role) => {
                if (role.gameObject.transform.position.y < 0) {
                    role.DropBeHurt(50, new Vector2(3, 3));
                }
                worldDomain.RoleDomain.TickHUD(role, dt);
            });

            CleanupRole();
        }

        void ApplyBasicLogic(float dt) {
            var roleDomain = worldDomain.RoleDomain;
            var roleFSMDomain = worldDomain.RoleFSMDomain;
            var stateEntity = worldContext.StateEntity;
            var roleRepo = worldContext.RoleRepo;
            roleRepo.ForeachAll((role) => {
                // AI
                if (role.ControlType == RoleControlType.AI) {
                    role.AIStrategy.Tick(dt);
                }

                // Role FSM
                roleFSMDomain.Tick(role, dt);
            });

            // Physics
            var phxDomain = worldDomain.WorldPhysicsDomain;
            phxDomain.Tick(dt);

        }

        void CleanupRole() {
            var roleRepo = worldContext.RoleRepo;

            List<int> removeList = new List<int>(roleRepo.Count);
            roleRepo.ForeachAll((role) => {
                if (role.FSMCom.Status == RoleFSMStatus.Dead) {
                    role.TearDown();
                    removeList.Add(role.ID);
                }
            });

            var count = removeList.Count;
            for (int i = 0; i < count; i += 1) {
                roleRepo.RemoveByID(removeList[i]);
            }
        }

    }
}
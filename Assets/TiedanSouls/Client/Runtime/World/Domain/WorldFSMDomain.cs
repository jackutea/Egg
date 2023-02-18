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
            // Physics
            Physics2D.IgnoreLayerCollision(LayerCollection.ROLE, LayerCollection.ROLE, true);

            var gameConfigTM = infraContext.TemplateCore.GameConfigTM;
            var firstFieldTypeID = gameConfigTM.hallFieldTypeID;
            var fieldDomain = worldDomain.FieldDomain;
            var field = fieldDomain.SpawnField(firstFieldTypeID);

            // Check
            var fieldType = field.fieldType;
            if (fieldType != FieldType.Hall) {
                TDLog.Error($"EnterState_Hall Error: {fieldType}");
                return;
            }

            // Check
            var spawnModelArray = field.SpawnModelArray;
            var arryLen = spawnModelArray?.Length;
            if (arryLen == 0) {
                TDLog.Error($"EnterState_Hall Error: SpawnModelArray Cant Be Empty!");
                return;
            }

            // Spawn Owner Role According To Field Template
            var spawnModel = spawnModelArray[0];
            var ownerRoleSpawnPos = spawnModel.pos;
            var roleDomain = worldDomain.RoleDomain;
            var owner = roleDomain.SpawnRole(RoleControlType.Player, 1000, AllyCollection.PLAYER, ownerRoleSpawnPos);

            // Set Camera 
            var cameraSetter = infraContext.CameraCore.SetterAPI;
            cameraSetter.Follow_Current(owner.transform, new Vector3(0, 0, -10), EasingType.Immediate, 1f, EasingType.Linear, 1f);
            cameraSetter.Confiner_Set_Current(true, field.transform.position, (Vector2)field.transform.position + field.ConfinerSize);

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
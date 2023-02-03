using System;
using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;

namespace TiedanSouls.World.Domain {

    public class WorldGameDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldDomain worldDomain;

        public WorldGameDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void EnterGame() {

            // ==== Physics ====
            Physics2D.IgnoreLayerCollision(LayerCollection.ROLE, LayerCollection.ROLE, true);

            // ==== Spawn ====
            var fieldDomain = worldDomain.FieldDomain;
            var field = fieldDomain.SpawnField();

            var roleDomain = worldDomain.RoleDomain;
            var owner = roleDomain.SpawnRole(RoleControlType.Player, 1000, AllyCollection.PLAYER, new Vector2(3, 3));
            _ = roleDomain.SpawnRole(RoleControlType.AI, 100_000, AllyCollection.ENEMY, new Vector2(5, 5));

            // ==== Camera ====
            var cameraSetter = infraContext.CameraCore.SetterAPI;
            cameraSetter.Follow_Current(owner.transform, new Vector3(0, 0, -10), EasingType.Immediate, 1f, EasingType.Linear, 1f);
            cameraSetter.Confiner_Set_Current(true, field.transform.position, (Vector2)field.transform.position + field.ConfinerSize);

            var stateEntity = worldContext.StateEntity;
            stateEntity.ownerRoleID = owner.ID;
            stateEntity.isRunning = true;

        }

        public void TickGameLoop(float dt) {

            var stateEntity = worldContext.StateEntity;
            if (!stateEntity.isRunning) {
                return;
            }

            var roleDomain = worldDomain.RoleDomain;
            var roleFSMDomain = worldDomain.RoleFSMDomain;
            var phxDomain = worldDomain.WorldPhysicsDomain;

            var roleRepo = worldContext.RoleRepo;
            var allRole = roleRepo.GetAll();

            foreach (var role in allRole) {

                // Process Input
                if (role.ID == stateEntity.ownerRoleID) {
                    roleDomain.RecordOwnerInput(role);
                }

                if (role.ControlType == RoleControlType.AI) {
                    role.AIStrategy.Tick(dt);
                }


                // Process Logic
                roleFSMDomain.Tick(role, dt);

            }

            // Process Logic
            phxDomain.Tick(dt);

            foreach (var role in allRole) {

                if (role.gameObject.transform.position.y < 0) {
                    role.DropBeHurt(50, new Vector2(3, 3));
                }

            }
            
            CleanupRole();

            // Process Render

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
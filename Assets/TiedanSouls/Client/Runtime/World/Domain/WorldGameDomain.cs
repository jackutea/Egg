using UnityEngine;
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
            fieldDomain.SpawnField();

            var roleDomain = worldDomain.RoleDomain;
            int ownerID = roleDomain.SpawnRole(AllyCollection.PLAYER, new Vector2(3, 3));

            int enemyID = roleDomain.SpawnRole(AllyCollection.ENEMY, new Vector2(5, 5));

            var stateEntity = worldContext.StateEntity;
            stateEntity.ownerRoleID = ownerID;
            stateEntity.isRunning = true;

        }

        public void TickGameLoop() {

            var stateEntity = worldContext.StateEntity;
            if (!stateEntity.isRunning) {
                return;
            }

            var roleDomain = worldDomain.RoleDomain;

            var roleRepo = worldContext.RoleRepo;
            var allRole = roleRepo.GetAll();

            // Process Input
            foreach (var role in allRole) {
                if (role.ID == stateEntity.ownerRoleID) {
                    roleDomain.RecordOwnerInput(role);
                }
                roleDomain.Move(role);
                roleDomain.Jump(role);
            }

            // Process Logic

            // Process Render

        }

    }
}
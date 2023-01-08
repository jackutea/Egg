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

            var fieldDomain = worldDomain.FieldDomain;
            fieldDomain.SpawnField();

            var roleDomain = worldDomain.RoleDomain;
            roleDomain.SpawnRole(new Vector2(3, 3));

        }

        public void TickGameLoop() {

            // Process Input

            // Process Logic

            // Process Render

        }

    }
}
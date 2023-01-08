using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;

namespace TiedanSouls.World.Controller {

    public class WorldController {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldDomain worldDomain;

        public WorldController() {
            worldContext = new WorldContext();
            worldDomain = new WorldDomain();
        }

        public void Inject(InfraContext infraContext) {

            this.infraContext = infraContext;

            worldContext.WorldFactory.Inject(infraContext);

            worldDomain.GameDomain.Inject(infraContext, worldContext, worldDomain);
            worldDomain.FieldDomain.Inject(infraContext, worldContext);
            worldDomain.RoleDomain.Inject(infraContext, worldContext);

        }

        public void Init() {
            infraContext.EventCenter.OnStartGameHandle += () => {
                worldDomain.GameDomain.EnterGame();
            };
        }

        public void FixedTick() {

        }

        public void Tick(float dt) {
            worldDomain.GameDomain.TickGameLoop();
        }

        public void LateTick(float dt) {

        }

    }

}
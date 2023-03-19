using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Controller {

    public class WorldController {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldDomain;

        public WorldController() {
            worldContext = new WorldContext();
            worldDomain = new WorldRootDomain();
        }

        public void Inject(InfraContext infraContext) {

            this.infraContext = infraContext;

            worldContext.WorldFactory.Inject(infraContext, worldContext);

            worldDomain.Inject(infraContext, worldContext);

        }

        public void Init() {
            infraContext.EventCenter.Listen_OnStartGameAct(() => {
                worldDomain.GameDomain.StartGame();
            });
        }

        float resTime;
        public void Tick(float dt) {
            worldDomain.RoleDomain.BackPlayerInput();

            resTime += dt;
            var logicIntervalTime = GameCollection.LOGIC_INTERVAL_TIME;
            while (resTime >= logicIntervalTime) {
                worldDomain.GameDomain.ApplyWorldState(logicIntervalTime);
                resTime -= logicIntervalTime;
                resTime = resTime < 0 ? 0 : resTime;
            }

            worldDomain.WorldRendererDomain.Tick(dt);
        }

    }

}
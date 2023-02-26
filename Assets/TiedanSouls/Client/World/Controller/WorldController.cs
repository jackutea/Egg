using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;

namespace TiedanSouls.World.Controller {

    public class WorldController {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldDomain worldDomain;

        float resTime;
        const float LOGIC_INTERVAL_TIME = 0.033f;

        public WorldController() {
            worldContext = new WorldContext();
            worldDomain = new WorldDomain();
        }

        public void Inject(InfraContext infraContext) {

            this.infraContext = infraContext;

            worldContext.WorldFactory.Inject(infraContext, worldContext);

            worldDomain.GameDomain.Inject(infraContext, worldContext, worldDomain);
            worldDomain.FieldDomain.Inject(infraContext, worldContext);
            worldDomain.RoleDomain.Inject(infraContext, worldContext);
            worldDomain.RoleFSMDomain.Inject(infraContext, worldContext, worldDomain);
            worldDomain.WorldPhysicsDomain.Inject(infraContext, worldContext, worldDomain);

        }

        public void Init() {
            infraContext.EventCenter.OnStartGameHandle += () => {
                worldDomain.GameDomain.EnterLobby();
            };
        }

        public void Tick(float dt) {
            worldDomain.RoleDomain.BackPlayerRInput();

            // Fixed Tick
            resTime += dt;
            while (resTime >= LOGIC_INTERVAL_TIME) {
                resTime -= LOGIC_INTERVAL_TIME;
                worldDomain.GameDomain.ApplyWorldState(LOGIC_INTERVAL_TIME);
            }

        }

    }

}
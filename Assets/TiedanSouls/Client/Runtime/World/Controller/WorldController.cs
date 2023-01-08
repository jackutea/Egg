using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;

namespace TiedanSouls.World.Controller {

    public class WorldController {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldController() {
            worldContext = new WorldContext();
        }

        public void Inject(InfraContext infraContext) {

            this.infraContext = infraContext;

            worldContext.WorldFactory.Inject(infraContext);

        }

        public void Init() {
            infraContext.EventCenter.OnStartGameHandle += () => {
                TDLog.Log("WorldController: StartGame");
                var field = worldContext.WorldFactory.CreateFieldEntity();

                var role = worldContext.WorldFactory.CreateRoleEntity();
                role.SetPos(new Vector2(3, 5));
            };
        }

    }

}
using UnityEngine;
using TiedanSouls.Infra.Facades;

namespace TiedanSouls.World.Controller {

    public class WorldController {

        InfraContext infraContext;

        public WorldController() { }

        public void Inject(InfraContext infraContext) {
            this.infraContext = infraContext;
        }

        public void Init() {
            infraContext.EventCenter.OnStartGameHandle += () => {
                TDLog.Log("WorldController: StartGame");
            };
        }

    }

}
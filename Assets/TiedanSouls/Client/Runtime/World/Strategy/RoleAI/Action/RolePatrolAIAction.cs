using GameArki.BTTreeNS;
using TiedanSouls.World.Facades;

namespace TiedanSouls.World.Entities {

    public class RolePatrolAIAction : IBTTreeAction {

        RoleEntity role;
        WorldContext worldContext;

        public RolePatrolAIAction() { }

        public void Inject(RoleEntity role, WorldContext worldContext) {
            this.role = role;
            this.worldContext = worldContext;
        }

        void IBTTreeAction.Enter() {
            TDLog.Log("RolePatrolAIAction Enter");
        }

        bool IBTTreeAction.Execute() {
            return true;
        }

        void IBTTreeAction.Exit() {
            TDLog.Log("RolePatrolAIAction exit");
        }

    }

}
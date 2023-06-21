using GameArki.BTTreeNS;
using TiedanSouls.Client.Facades;

namespace TiedanSouls.Client.Entities {

    public class RoleNothingAIPrecondition : IBTTreePrecondition {

        RoleEntity role;
        WorldContext worldContext;

        public RoleNothingAIPrecondition() { }

        public void Inject(RoleEntity role, WorldContext worldContext) {
            this.role = role;
            this.worldContext = worldContext;
        }

        bool IBTTreePrecondition.CanEnter() {
            return true;
        }

    }

}
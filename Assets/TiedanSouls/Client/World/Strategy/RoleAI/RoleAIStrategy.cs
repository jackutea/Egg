using GameArki.BTTreeNS;

namespace TiedanSouls.World.Entities {

    public class RoleAIStrategy {

        BTTree bt;

        public RoleAIStrategy() { }

        public void Inject(BTTree bt) {
            this.bt = bt;
        }

        public void Activate() {
            bt.Activate();
        }

        public void Tick(float dt) {
            bt.Tick();
        }

    }

}
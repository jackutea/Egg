namespace TiedanSouls.World.Entities {

    public class RoleFSMComponent {

        RoleFSMStatus status;
        public RoleFSMStatus Status => status;

        public RoleFSMComponent() {}

        public void EnterIdle() {
            status = RoleFSMStatus.Idle;
        }

    }
}
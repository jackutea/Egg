namespace TiedanSouls.World.Entities {

    public class RoleFSMComponent {

        RoleFSMStatus status;
        public RoleFSMStatus Status => status;

        RoleCastingStateModel castingState;
        public RoleCastingStateModel CastingState => castingState;

        public RoleFSMComponent() {
            status = RoleFSMStatus.Idle;
            castingState = new RoleCastingStateModel();
        }

        public void EnterIdle() {
            status = RoleFSMStatus.Idle;
        }

        public void EnterCasting(SkillorModel skillorModel) {
            status = RoleFSMStatus.Casting;
            castingState.castingSkillor = skillorModel;
        }

    }
}
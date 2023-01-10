using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleFSMComponent {

        RoleFSMStatus status;
        public RoleFSMStatus Status => status;

        RoleCastingStateModel castingState;
        public RoleCastingStateModel CastingState => castingState;

        RoleBeHurtStateModel beHurtState;
        public RoleBeHurtStateModel BeHurtState => beHurtState;

        public RoleFSMComponent() {
            status = RoleFSMStatus.Idle;
            castingState = new RoleCastingStateModel();
            beHurtState = new RoleBeHurtStateModel();
        }

        public void EnterIdle() {
            status = RoleFSMStatus.Idle;
        }

        public void EnterCasting(SkillorModel skillorModel) {
            status = RoleFSMStatus.Casting;
            castingState.castingSkillor = skillorModel;
        }

        public void EnterBeHurt(int stunFrame, Vector2 knockForce) {
            status = RoleFSMStatus.BeHurt;
            beHurtState.stunFrame = stunFrame;
            beHurtState.knockForce = knockForce;
        }

    }
}
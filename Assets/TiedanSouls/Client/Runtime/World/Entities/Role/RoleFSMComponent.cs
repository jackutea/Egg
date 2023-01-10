using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleFSMComponent {

        RoleFSMStatus status;
        public RoleFSMStatus Status => status;

        RoleCastingStateModel castingState;
        public RoleCastingStateModel CastingState => castingState;

        RoleBeHurtStateModel beHurtState;
        public RoleBeHurtStateModel BeHurtState => beHurtState;

        RoleDeadStateModel deadState;
        public RoleDeadStateModel DeadState => deadState;

        public RoleFSMComponent() {
            status = RoleFSMStatus.Idle;
            castingState = new RoleCastingStateModel();
            beHurtState = new RoleBeHurtStateModel();
            deadState = new RoleDeadStateModel();
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

        public void EnterDead() {
            status = RoleFSMStatus.Dead;
        }

    }
}
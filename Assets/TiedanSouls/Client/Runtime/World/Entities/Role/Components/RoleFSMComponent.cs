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
            var stateModel = castingState;
            stateModel.castingSkillor = skillorModel;
            stateModel.isEntering = true;
        }

        public void EnterBeHurt(Vector2 fromPos, HitPowerModel hitPowerModel) {
            status = RoleFSMStatus.BeHurt;
            var stateModel = beHurtState;
            stateModel.fromPos = fromPos;
            stateModel.knockbackForce = hitPowerModel.knockbackForce;
            stateModel.knockbackFrame = hitPowerModel.knockbackFrame;
            stateModel.hitStunFrame = hitPowerModel.hitStunFrame;
            stateModel.curFrame = 0;
            stateModel.isEnter = true;
        }

        public void EnterDead() {
            status = RoleFSMStatus.Dead;
        }

    }
}
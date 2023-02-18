using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleFSMComponent {

        RoleFSMStatus status;
        public RoleFSMStatus Status => status;

        RoleIdleStateModel idleState;
        public RoleIdleStateModel IdleState => idleState;

        RoleCastingStateModel castingState;
        public RoleCastingStateModel CastingState => castingState;

        RoleBeHurtStateModel beHurtState;
        public RoleBeHurtStateModel BeHurtState => beHurtState;

        RoleDeadStateModel deadState;
        public RoleDeadStateModel DeadState => deadState;

        public RoleFSMComponent() {
            status = RoleFSMStatus.Idle;
            idleState = new RoleIdleStateModel();
            castingState = new RoleCastingStateModel();
            beHurtState = new RoleBeHurtStateModel();
            deadState = new RoleDeadStateModel();
        }

        public void EnterIdle() {
            status = RoleFSMStatus.Idle;
            idleState.isEnter = true;
            TDLog.Log("RoleFSM: EnterIdle");
        }

        public void EnterCasting(SkillorModel skillorModel) {
            status = RoleFSMStatus.Casting;
            var stateModel = castingState;
            stateModel.castingSkillor = skillorModel;
            stateModel.isEntering = true;
            TDLog.Log("RoleFSM: EnterCasting");
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
            TDLog.Log("RoleFSM: EnterBeHurt");
        }

        public void EnterDead() {
            status = RoleFSMStatus.Dead;
            TDLog.Log("RoleFSM: EnterDead");
        }

    }
}
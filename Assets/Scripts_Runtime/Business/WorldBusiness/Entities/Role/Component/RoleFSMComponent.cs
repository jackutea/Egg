using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleFSMComponent {

        #region [状态]

        RoleFSMState fsmState;
        public RoleFSMState FSMState => fsmState;

        RoleIdleStateModel idleStateModel;
        public RoleIdleStateModel IdleStateModel => idleStateModel;

        RoleCastingStateModel castingStateModel;
        public RoleCastingStateModel CastingStateModel => castingStateModel;

        RoleBeHitStateModel beHitStateModel;
        public RoleBeHitStateModel BeHitStateModel => beHitStateModel;

        RoleDyingStateModel dyingStateModel;
        public RoleDyingStateModel DyingStateModel => dyingStateModel;

        #endregion

        public RoleFSMComponent() {
            idleStateModel = new RoleIdleStateModel();
            castingStateModel = new RoleCastingStateModel();
            beHitStateModel = new RoleBeHitStateModel();
            dyingStateModel = new RoleDyingStateModel();
        }

        public void ResetAll() {
            ResetFSMState();
        }

        public void ResetFSMState() {
            idleStateModel.Reset();
            castingStateModel.Reset();
            beHitStateModel.Reset();
            dyingStateModel.Reset();
            fsmState = RoleFSMState.None;
        }

        #region [状态]

        public void Enter_Idle() {
            var stateModel = idleStateModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            fsmState = RoleFSMState.Idle;
            TDLog.Log($"角色 状态 - 设置 '{fsmState}'");
        }


        public void Enter_Casting(SkillEntity skill, Vector2 chosedPoint) {
            var stateModel = castingStateModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            stateModel.SetCastingSkill(skill);
            stateModel.SetChosedPoint(chosedPoint);
            fsmState = RoleFSMState.Casting;
            TDLog.Log($"角色 状态 - 切换  {fsmState} {skill.IDCom.TypeID} / 选择点 {chosedPoint}");
        }

        public void Enter_BeHit(Vector3 beHitDir, in BeHitModel beHitModel) {
            var stateModel = beHitStateModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            stateModel.SetBeHitDir(beHitDir);
            stateModel.SetMaintainFrame(beHitModel.maintainFrame);
            stateModel.SetKnockBackSpeedArray(beHitModel.knockBackSpeedArray.Clone() as float[]);
            stateModel.SetKnockUpSpeedArray(beHitModel.knockUpSpeedArray.Clone() as float[]);
            fsmState = RoleFSMState.BeHit;
            TDLog.Log($"角色 状态 - 切换 '{fsmState}'");
        }

        public void Enter_Dying(int maintainFrame) {
            var stateModel = this.dyingStateModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.maintainFrame = maintainFrame;

            fsmState = RoleFSMState.Dying;
            TDLog.Log($"角色 状态 - 切换 '{fsmState}'");
        }

        #endregion

    }

}
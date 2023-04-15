using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleFSMComponent {

        #region [状态]

        RoleFSMState fsmState;
        public RoleFSMState FSMState => fsmState;

        RoleIdleStateModel idleStateModel;
        public RoleIdleStateModel IdleStateModel => idleStateModel;

        RoleJumpingUpStateModel jumpingUpStateModel;
        public RoleJumpingUpStateModel JumpingUpStateModel => jumpingUpStateModel;

        RoleCastingStateModel castingStateModel;
        public RoleCastingStateModel CastingStateModel => castingStateModel;

        RoleBeHitStateModel beHitStateModel;
        public RoleBeHitStateModel BeHitStateModel => beHitStateModel;

        RoleDyingStateModel dyingStateModel;
        public RoleDyingStateModel DyingStateModel => dyingStateModel;

        #endregion

        #region [控制状态]

        RoleStateModel_KnockBack knockBackModel;
        public RoleStateModel_KnockBack KnockBackModel => knockBackModel;

        RoleStateModel_KnockUp knockUpModel;
        public RoleStateModel_KnockUp KnockUpModel => knockUpModel;

        #endregion

        #region [角色位置状态]

        RolePositionStatus positionStatus;
        public RolePositionStatus PositionStatus => positionStatus;

        RoleStateModel_OnGround onGroundModel;
        public RoleStateModel_OnGround OnGroundModel => onGroundModel;

        RoleStateModel_OnCrossPlatform onCrossPlatformStateModel;
        public RoleStateModel_OnCrossPlatform OnCrossPlatformStateModel => onCrossPlatformStateModel;

        RoleInWaterStateModel inWaterStateModel;
        public RoleInWaterStateModel InWaterStateModel => inWaterStateModel;

        #endregion

        public RoleFSMComponent() {
            idleStateModel = new RoleIdleStateModel();
            jumpingUpStateModel = new RoleJumpingUpStateModel();
            castingStateModel = new RoleCastingStateModel();
            beHitStateModel = new RoleBeHitStateModel();
            knockBackModel = new RoleStateModel_KnockBack();
            knockUpModel = new RoleStateModel_KnockUp();
            dyingStateModel = new RoleDyingStateModel();

            onGroundModel = new RoleStateModel_OnGround();
            onCrossPlatformStateModel = new RoleStateModel_OnCrossPlatform();
            inWaterStateModel = new RoleInWaterStateModel();
        }

        public void ResetAll() {
            ResetFSMState();
            ResetCtrlStatus();
            ResetPositionStatus();
        }

        public void ResetFSMState() {
            idleStateModel.Reset();
            castingStateModel.Reset();
            beHitStateModel.Reset();
            dyingStateModel.Reset();
            fsmState = RoleFSMState.None;
        }

        public void ResetCtrlStatus() {
            knockBackModel.Reset();
            knockUpModel.Reset();
        }

        public void ResetPositionStatus() {
            onGroundModel.Reset();
            onCrossPlatformStateModel.Reset();
            inWaterStateModel.Reset();
            positionStatus = RolePositionStatus.None;
        }

        #region [状态]

        public void Enter_Idle() {
            var stateModel = idleStateModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            fsmState = RoleFSMState.Idle;
            TDLog.Log($"角色 状态 - 设置 '{fsmState}'");
        }


        public void Enter_Casting(SkillEntity skill, bool isCombo, Vector2 chosedPoint) {
            var stateModel = castingStateModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            stateModel.SetCastingSkill(skill);
            stateModel.SetIsCombo(isCombo);
            stateModel.SetChosedPoint(chosedPoint);
            fsmState = RoleFSMState.Casting;
            TDLog.Log($"角色 状态 - 切换  {fsmState} {skill.IDCom.TypeID} / 是否连招 {isCombo} / 选择点 {chosedPoint}");
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

        public void Enter_JumpingDown() {
            fsmState = RoleFSMState.JumpingDown;
            TDLog.Log($"角色 状态 - 切换 '{fsmState}'");
        }

        public void Enter_JumpingUp() {
            fsmState = RoleFSMState.JumpingUp;
            var stateModel = this.jumpingUpStateModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            TDLog.Log($"角色 状态 - 切换 '{fsmState}'");
        }

        public void Enter_Falling() {
            fsmState = RoleFSMState.Falling;
            TDLog.Log($"角色 状态 - 切换 '{fsmState}'");
        }

        #endregion

        #region [位置状态]

        public void AddPositionStatus_OnGround() {
            var stateModel = onGroundModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            this.positionStatus = this.positionStatus.AddStatus(RolePositionStatus.OnGround);
            TDLog.Log($"角色 位置状态 - 添加  '{RolePositionStatus.OnGround}'\n{positionStatus.GetString()}");
        }

        public void AddPositionStatus_StandInCrossPlatform() {
            var stateModel = onCrossPlatformStateModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            this.positionStatus = this.positionStatus.AddStatus(RolePositionStatus.OnCrossPlatform);
            TDLog.Log($"角色 位置状态 - 添加  '{RolePositionStatus.OnCrossPlatform}'\n{positionStatus.GetString()}");
        }

        public void AddPositionStatus_StandInWater() {
            var stateModel = inWaterStateModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            this.positionStatus = this.positionStatus.AddStatus(RolePositionStatus.InWater);
            TDLog.Log($"角色 位置状态 - 添加  '{RolePositionStatus.InWater}'\n{positionStatus.GetString()}");
        }

        public void RemovePositionStatus_OnGround() {
            this.positionStatus = positionStatus.RemoveStatus(RolePositionStatus.OnGround);
            TDLog.Log($"角色 位置状态 - 移除  '{RolePositionStatus.OnGround}'\n{positionStatus.GetString()}");
        }

        public void RemovePositionStatus_StandInCrossPlatform() {
            this.positionStatus = positionStatus.RemoveStatus(RolePositionStatus.OnCrossPlatform);
            TDLog.Log($"角色 位置状态 - 移除  '{RolePositionStatus.OnCrossPlatform}'\n{positionStatus.GetString()}");
        }

        public void RemovePositionStatus_StandInWater() {
            this.positionStatus = positionStatus.RemoveStatus(RolePositionStatus.InWater);
            TDLog.Log($"角色 位置状态 - 移除  '{RolePositionStatus.InWater}'\n{positionStatus.GetString()}");
        }

        #endregion

    }

}
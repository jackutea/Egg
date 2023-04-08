using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleFSMComponent {

        #region [状态]

        RoleActionState actionState;
        public RoleActionState ActionState => actionState;

        RoleFSMModel_Idle idleModel;
        public RoleFSMModel_Idle IdleModel => idleModel;

        RoleFSMModel_JumpingUp jumpingUpModel;
        public RoleFSMModel_JumpingUp JumpingUpModel => jumpingUpModel;

        RoleFSMModel_Casting castingModel;
        public RoleFSMModel_Casting CastingModel => castingModel;

        RoleFSMModel_SkillMove skillMoveModel;
        public RoleFSMModel_SkillMove SkillMoveModel => skillMoveModel;

        RoleFSMModel_Dying dyingModel;
        public RoleFSMModel_Dying DyingModel => dyingModel;

        #endregion

        #region [控制状态]

        RoleCtrlStatus ctrlStatus;
        public RoleCtrlStatus CtrlStatus => ctrlStatus;

        RoleFSMModel_KnockBack knockBackModel;
        public RoleFSMModel_KnockBack KnockBackModel => knockBackModel;

        RoleFSMModel_KnockUp knockUpModel;
        public RoleFSMModel_KnockUp KnockUpModel => knockUpModel;

        #endregion

        #region [角色位置状态]

        RolePositionStatus positionStatus;
        public RolePositionStatus PositionStatus => positionStatus;

        RoleFSMModel_OnGround onGroundModel;
        public RoleFSMModel_OnGround OnGroundModel => onGroundModel;

        RoleFSMModel_StandInCrossPlatform standInPlatformModel;
        public RoleFSMModel_StandInCrossPlatform StandInCrossPlatformModel => standInPlatformModel;

        RoleFSMModel_StandInWater standInWaterModel;
        public RoleFSMModel_StandInWater StandInWaterModel => standInWaterModel;

        #endregion

        public RoleFSMComponent() {
            idleModel = new RoleFSMModel_Idle();
            jumpingUpModel = new RoleFSMModel_JumpingUp();
            castingModel = new RoleFSMModel_Casting();
            skillMoveModel = new RoleFSMModel_SkillMove();
            knockBackModel = new RoleFSMModel_KnockBack();
            knockUpModel = new RoleFSMModel_KnockUp();
            dyingModel = new RoleFSMModel_Dying();

            onGroundModel = new RoleFSMModel_OnGround();
            standInPlatformModel = new RoleFSMModel_StandInCrossPlatform();
            standInWaterModel = new RoleFSMModel_StandInWater();
        }

        public void ResetAll() {
            ResetActionState();
            ResetCtrlStatus();
            ResetPositionStatus();
        }

        public void ResetActionState() {
            idleModel.Reset();
            castingModel.Reset();
            skillMoveModel.Reset();
            dyingModel.Reset();
            actionState = RoleActionState.None;
        }

        public void ResetCtrlStatus() {
            knockBackModel.Reset();
            knockUpModel.Reset();
            ctrlStatus = RoleCtrlStatus.None;
        }

        public void ResetPositionStatus() {
            onGroundModel.Reset();
            standInPlatformModel.Reset();
            standInWaterModel.Reset();
            positionStatus = RolePositionStatus.None;
        }

        #region [状态]

        public void Enter_Idle() {
            var stateModel = idleModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            actionState = RoleActionState.Idle;
            TDLog.Log($"角色 状态 - 设置 '{actionState}'");
        }


        public void Enter_Cast(int skillTypeID, bool isCombo, Vector2 chosedPoint) {
            var stateModel = castingModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.SetCastingSkillTypeID(skillTypeID);
            stateModel.SetIsCombo(isCombo);
            stateModel.SetChosedPoint(chosedPoint);

            actionState = RoleActionState.Casting;
            TDLog.Log($"角色 状态 - 切换  {actionState} {skillTypeID} / 是否连招 {isCombo} / 选择点 {chosedPoint}\n{ctrlStatus.GetString()}");
        }

        public void Enter_Dying(int maintainFrame) {
            var stateModel = this.dyingModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.maintainFrame = maintainFrame;

            actionState = RoleActionState.Dying;
            TDLog.Log($"角色 状态 - 切换 '{actionState}'");
        }

        public void Enter_JumpingDown() {
            actionState = RoleActionState.JumpingDown;
            TDLog.Log($"角色 状态 - 切换 '{actionState}'");
        }

        public void Enter_JumpingUp() {
            actionState = RoleActionState.JumpingUp;
            var stateModel = this.jumpingUpModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            TDLog.Log($"角色 状态 - 切换 '{actionState}'");
        }

        public void Enter_Falling() {
            actionState = RoleActionState.Falling;
            TDLog.Log($"角色 状态 - 切换 '{actionState}'");
        }

        #endregion

        #region [控制状态]

        public void AddCtrlStatus_KnockBack(Vector2 beHitDir, in KnockBackModel model) {
            var stateModel = this.knockBackModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.beHitDir = beHitDir;
            stateModel.knockBackSpeedArray = model.knockBackSpeedArray;

            ctrlStatus = ctrlStatus.AddStatus(RoleCtrlStatus.KnockBack);
            TDLog.Log($"角色 控制状态 - 添加  '{RoleCtrlStatus.KnockBack}'\n{ctrlStatus.GetString()}");
        }

        public void RemoveCtrlStatus_KnockBack() {
            ctrlStatus = ctrlStatus.RemoveStatus(RoleCtrlStatus.KnockBack);
            TDLog.Log($"角色 控制状态 - 移除  '{RoleCtrlStatus.KnockBack}'\n{ctrlStatus.GetString()}");
        }

        public void AddCtrlStatus_KnockUp(in KnockUpModel model) {
            var stateModel = this.knockUpModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.knockUpSpeedArray = model.knockUpSpeedArray;

            ctrlStatus = ctrlStatus.AddStatus(RoleCtrlStatus.KnockUp);
            TDLog.Log($"角色 控制状态 - 添加  '{RoleCtrlStatus.KnockUp}'\n{ctrlStatus.GetString()}");
        }

        public void RemoveCtrlStatus_KnockUp() {
            ctrlStatus = ctrlStatus.RemoveStatus(RoleCtrlStatus.KnockUp);
            TDLog.Log($"角色 控制状态 - 移除  '{RoleCtrlStatus.KnockUp}'\n{ctrlStatus.GetString()}");
        }

        public void AddCtrlStatus_SkillMove(SkillMoveCurveModel skilMoveCurveModel) {
            var stateModel = skillMoveModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            stateModel.SetIsFaceTo(skilMoveCurveModel.isFaceTo);
            stateModel.SetNeedWaitForMoveEnd(skilMoveCurveModel.needWaitForMoveEnd);
            var moveCurveModel = skilMoveCurveModel.moveCurveModel;
            stateModel.SetMoveSpeedArray(moveCurveModel.moveSpeedArray.Clone() as float[]);
            stateModel.SetMoveDirArray(moveCurveModel.moveDirArray.Clone() as Vector3[]);

            ctrlStatus = ctrlStatus.AddStatus(RoleCtrlStatus.SkillMove);
            TDLog.Log($"角色 控制状态 - 添加  '{RoleCtrlStatus.SkillMove}'\n{ctrlStatus.GetString()}");
        }

        public void RemoveCtrlStatus_SkillMove() {
            ctrlStatus = ctrlStatus.RemoveStatus(RoleCtrlStatus.SkillMove);
            TDLog.Log($"角色 控制状态 - 移除  '{RoleCtrlStatus.SkillMove}'\n{ctrlStatus.GetString()}");
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
            var stateModel = standInPlatformModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            this.positionStatus = this.positionStatus.AddStatus(RolePositionStatus.OnCrossPlatform);
            TDLog.Log($"角色 位置状态 - 添加  '{RolePositionStatus.OnCrossPlatform}'\n{positionStatus.GetString()}");
        }

        public void AddPositionStatus_StandInWater() {
            var stateModel = standInWaterModel;
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

        #region [Locomotion 判断]

        /// <summary>
        /// 是否可以移动
        /// </summary>
        public bool Can_Move() {
            return actionState != RoleActionState.Dying
                && actionState != RoleActionState.JumpingDown
                && !ctrlStatus.Contains(RoleCtrlStatus.SkillMove)
                && !ctrlStatus.Contains(RoleCtrlStatus.KnockBack)
                && !ctrlStatus.Contains(RoleCtrlStatus.KnockUp)
                && !ctrlStatus.Contains(RoleCtrlStatus.Root)
                && !ctrlStatus.Contains(RoleCtrlStatus.Stun);
        }

        /// <summary>
        /// 是否可以下跳
        /// </summary>
        public bool CanJumpDown() {
            return actionState != RoleActionState.Dying
                && actionState != RoleActionState.JumpingUp
                && !ctrlStatus.Contains(RoleCtrlStatus.KnockBack)
                && !ctrlStatus.Contains(RoleCtrlStatus.KnockUp)
                && !ctrlStatus.Contains(RoleCtrlStatus.Root)
                && !ctrlStatus.Contains(RoleCtrlStatus.Stun)
                && positionStatus.Contains(RolePositionStatus.OnCrossPlatform);
        }

        /// <summary>
        /// 是否可以上跳
        /// </summary>
        public bool CanJumpUp() {
            return actionState != RoleActionState.Dying
                && actionState != RoleActionState.JumpingUp
                && !ctrlStatus.Contains(RoleCtrlStatus.KnockBack)
                && !ctrlStatus.Contains(RoleCtrlStatus.KnockUp)
                && !ctrlStatus.Contains(RoleCtrlStatus.Root)
                && !ctrlStatus.Contains(RoleCtrlStatus.Stun)
                && (positionStatus.Contains(RolePositionStatus.OnGround)
                    || positionStatus.Contains(RolePositionStatus.OnCrossPlatform)
                    || positionStatus.Contains(RolePositionStatus.InWater));
        }

        /// <summary>
        /// 是否会下落
        /// </summary>
        public bool CanFall() {
            return !ctrlStatus.Contains(RoleCtrlStatus.KnockBack)
                && !ctrlStatus.Contains(RoleCtrlStatus.KnockUp)
                && !ctrlStatus.Contains(RoleCtrlStatus.Stun)
                && !positionStatus.Contains(RolePositionStatus.OnGround)
                && !positionStatus.Contains(RolePositionStatus.OnCrossPlatform);
        }

        /// <summary>
        /// 是否可改变面向
        /// </summary>
        public bool CanChangeFaceTo() {
            return actionState != RoleActionState.Dying
                && !ctrlStatus.Contains(RoleCtrlStatus.KnockBack)
                && !ctrlStatus.Contains(RoleCtrlStatus.KnockUp)
                && !ctrlStatus.Contains(RoleCtrlStatus.Root)
                && !ctrlStatus.Contains(RoleCtrlStatus.Stun);
        }

        #endregion

    }

}
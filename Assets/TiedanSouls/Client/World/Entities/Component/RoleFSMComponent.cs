using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleFSMComponent {

        RoleStateFlag stateFlag;
        public RoleStateFlag StateFlag => stateFlag;

        RoleFSMModel_Idle idleModel;
        public RoleFSMModel_Idle IdleModel => idleModel;

        RoleFSMModel_Cast castingModel;
        public RoleFSMModel_Cast CastingModel => castingModel;

        RoleFSMModel_SkillMove skillMoveModel;
        public RoleFSMModel_SkillMove SkillMoveModel => skillMoveModel;

        RoleFSMModel_KnockBack knockBackModel;
        public RoleFSMModel_KnockBack KnockBackModel => knockBackModel;

        RoleFSMModel_KnockUp knockUpModel;
        public RoleFSMModel_KnockUp KnockUpModel => knockUpModel;

        RoleFSMModel_Dying dyingModel;
        public RoleFSMModel_Dying DyingModel => dyingModel;

        RoleFSMModel_StandInGround standInGroundModel;
        public RoleFSMModel_StandInGround StandInGroundModel => standInGroundModel;

        RoleFSMModel_StandInPlatform standInPlatformModel;
        public RoleFSMModel_StandInPlatform StandInPlatformModel => standInPlatformModel;

        RoleFSMModel_StandInWater standInWaterModel;
        public RoleFSMModel_StandInWater StandInWaterModel => standInWaterModel;

        RoleFSMModel_LeaveGround leaveGroundModel;
        public RoleFSMModel_LeaveGround LeaveGroundModel => leaveGroundModel;

        RoleFSMModel_LeavePlatform leavePlatformModel;
        public RoleFSMModel_LeavePlatform LeavePlatformModel => leavePlatformModel;

        RoleFSMModel_LeaveWater leaveWaterModel;
        public RoleFSMModel_LeaveWater LeaveWaterModel => leaveWaterModel;

        bool isExited;
        public bool IsExited => isExited;
        public void SetIsExited(bool value) => isExited = value;

        public RoleFSMComponent() {
            idleModel = new RoleFSMModel_Idle();
            castingModel = new RoleFSMModel_Cast();
            skillMoveModel = new RoleFSMModel_SkillMove();
            knockBackModel = new RoleFSMModel_KnockBack();
            knockUpModel = new RoleFSMModel_KnockUp();
            dyingModel = new RoleFSMModel_Dying();

            standInGroundModel = new RoleFSMModel_StandInGround();
            standInPlatformModel = new RoleFSMModel_StandInPlatform();
            standInWaterModel = new RoleFSMModel_StandInWater();
            leaveGroundModel = new RoleFSMModel_LeaveGround();
            leavePlatformModel = new RoleFSMModel_LeavePlatform();
            leaveWaterModel = new RoleFSMModel_LeaveWater();

            SetIdle();
        }

        public void Reset() {
            isExited = false;
            idleModel.Reset();
            castingModel.Reset();
            knockBackModel.Reset();
            dyingModel.Reset();
            SetIdle();
        }

        public void SetIdle() {
            var stateModel = idleModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateFlag = RoleStateFlag.Idle;
            TDLog.Log($"角色状态机 - 设置 '{stateFlag}'");
        }

        #region [添加 状态标记]

        public void Add_Cast(int skillTypeID, bool isCombo, Vector2 chosedPoint) {
            var stateModel = castingModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.SetCastingSkillTypeID(skillTypeID);
            stateModel.SetIsCombo(isCombo);
            stateModel.SetChosedPoint(chosedPoint);

            stateFlag = stateFlag.AddStateFlag(RoleStateFlag.Cast);
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  {RoleStateFlag.Cast} {skillTypeID} / 是否连招 {isCombo} / 选择点 {chosedPoint}\n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_SkillMove(SkillMoveCurveModel skilMoveCurveModel) {
            var stateModel = skillMoveModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            stateModel.SetIsFaceTo(skilMoveCurveModel.isFaceTo);
            var moveCurveModel = skilMoveCurveModel.moveCurveModel;
            stateModel.SetMoveSpeedArray(moveCurveModel.moveSpeedArray.Clone() as float[]);
            stateModel.SetMoveDirArray(moveCurveModel.moveDirArray.Clone() as Vector3[]);

            stateFlag = stateFlag.AddStateFlag(RoleStateFlag.SkillMove);
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.SkillMove}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_KnockBack(Vector2 beHitDir, in KnockBackModel model) {
            var stateModel = this.knockBackModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.beHitDir = beHitDir;
            stateModel.knockBackSpeedArray = model.knockBackSpeedArray;

            stateFlag = stateFlag.AddStateFlag(RoleStateFlag.KnockBack);
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.KnockBack}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_KnockUp(in KnockUpModel model) {
            var stateModel = this.knockUpModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.knockUpSpeedArray = model.knockUpSpeedArray;

            stateFlag = stateFlag.AddStateFlag(RoleStateFlag.KnockUp);
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.KnockUp}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_Dying(int maintainFrame) {
            var stateModel = this.dyingModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.maintainFrame = maintainFrame;

            stateFlag = stateFlag.AddStateFlag(RoleStateFlag.Dying);
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.Dying}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_StandInGround() {
            var stateModel = standInGroundModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.StandInGround}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_StandInPlatform() {
            var stateModel = standInPlatformModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.StandInPlatform}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_StandInWater() {
            var stateModel = standInWaterModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.StandInWater}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_LeaveGround() {
            var stateModel = leaveGroundModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.LeaveGround}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_LeavePlatform() {
            var stateModel = leavePlatformModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.LeavePlatform}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void Add_LeaveWater() {
            var stateModel = leaveWaterModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            TDLog.Log($"角色状态机 - 添加  '{RoleStateFlag.LeaveWater}'  \n{stateFlag.ToString_AllFlags()}");
        }

        #endregion

        #region [移除 状态标记]

        public void Remove_Idle() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.Idle);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.Idle}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_Cast() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.Cast);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.Cast}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_SkillMove() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.SkillMove);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.SkillMove}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_KnockBack() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.KnockBack);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.KnockBack}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_KnockUp() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.KnockUp);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.KnockUp}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_StandInGround() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.StandInGround);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.StandInGround}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_StandInPlatform() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.StandInPlatform);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.StandInPlatform}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_StandInWater() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.StandInWater);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.StandInWater}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_LeaveGround() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.LeaveGround);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.LeaveGround}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_LeavePlatform() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.LeavePlatform);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.LeavePlatform}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void Remove_LeaveWater() {
            stateFlag = stateFlag.RemoveStateFlag(RoleStateFlag.LeaveWater);
            TDLog.Log($"角色状态机 - 移除  '{RoleStateFlag.LeaveWater}'\n{stateFlag.ToString_AllFlags()}");
        }

        #endregion

        #region [Locomotion 判断]

        /// <summary>
        /// 当前状态 是否可以移动
        /// </summary>
        public bool Can_Move() {
            return !stateFlag.Contains(RoleStateFlag.Dying)
                && !stateFlag.Contains(RoleStateFlag.SkillMove)
                && !stateFlag.Contains(RoleStateFlag.KnockBack)
                && !stateFlag.Contains(RoleStateFlag.KnockUp)
                && !stateFlag.Contains(RoleStateFlag.Root)
                && !stateFlag.Contains(RoleStateFlag.Stun);
        }

        /// <summary>
        /// 当前状态 是否可以旋转
        /// </summary>
        public bool Can_SkillMove() {
            return !stateFlag.Contains(RoleStateFlag.Dying)
                && !stateFlag.Contains(RoleStateFlag.KnockBack)
                && !stateFlag.Contains(RoleStateFlag.KnockUp)
                && !stateFlag.Contains(RoleStateFlag.Root)
                && !stateFlag.Contains(RoleStateFlag.Stun);
        }

        /// <summary>
        /// 当前状态 是否可以跳跃
        /// </summary>
        public bool Can_Jump() {
            return !stateFlag.Contains(RoleStateFlag.Dying)
                && !stateFlag.Contains(RoleStateFlag.KnockBack)
                && !stateFlag.Contains(RoleStateFlag.KnockUp)
                && !stateFlag.Contains(RoleStateFlag.Root)
                && !stateFlag.Contains(RoleStateFlag.Stun);
        }

        /// <summary>
        /// 当前状态 是否会下落
        /// </summary>
        public bool Can_Fall() {
            return !stateFlag.Contains(RoleStateFlag.KnockBack)
                && !stateFlag.Contains(RoleStateFlag.KnockUp)
                && !stateFlag.Contains(RoleStateFlag.Stun)
                && !stateFlag.Contains(RoleStateFlag.StandInGround)
                && !stateFlag.Contains(RoleStateFlag.StandInPlatform);
        }

        #endregion

        #region [施法 判断]

        /// <summary>
        /// 当前状态 是否可以释放 普通技能
        /// </summary>
        public bool Can_CastNormalSkill() {
            return !stateFlag.Contains(RoleStateFlag.Dying)
                && !stateFlag.Contains(RoleStateFlag.Root)
                && !stateFlag.Contains(RoleStateFlag.Stun)
                && !stateFlag.Contains(RoleStateFlag.Silence);
        }

        #endregion

        #region [Idle 判断]

        public bool NeedSetIdle() {
            return stateFlag == 0;
        }

        #endregion

    }
}
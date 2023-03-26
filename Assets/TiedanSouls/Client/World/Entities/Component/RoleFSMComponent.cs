using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleFSMComponent {

        StateFlag stateFlag;
        public StateFlag StateFlag => stateFlag;

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

            stateFlag = StateFlag.Idle;
            TDLog.Log($"角色状态机 - 设置 '{stateFlag}'");
        }

        #region [添加 状态标记]

        public void AddCast(int skillTypeID, bool isCombo, Vector2 chosedPoint) {
            var stateModel = castingModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.SetCastingSkillTypeID(skillTypeID);
            stateModel.SetIsCombo(isCombo);
            stateModel.SetChosedPoint(chosedPoint);

            stateFlag = stateFlag.AddStateFlag(StateFlag.Cast);
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  {StateFlag.Cast} {skillTypeID} / 是否连招 {isCombo} / 选择点 {chosedPoint}\n{stateFlag.ToString_AllFlags()}");
        }

        public void AddSkillMove(SkillMoveCurveModel skilMoveCurveModel) {
            var stateModel = skillMoveModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);
            stateModel.SetIsFaceTo(skilMoveCurveModel.isFaceTo);
            var moveCurveModel = skilMoveCurveModel.moveCurveModel;
            stateModel.SetMoveSpeedArray(moveCurveModel.moveSpeedArray.Clone() as float[]);
            stateModel.SetMoveDirArray(moveCurveModel.moveDirArray.Clone() as Vector3[]);

            stateFlag = stateFlag.AddStateFlag(StateFlag.SkillMove);
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  '{StateFlag.SkillMove}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void AddKnockBack(Vector2 beHitDir, in KnockBackModel model) {
            var knockBackSpeedArray = model.knockBackSpeedArray;
            if (knockBackSpeedArray == null || knockBackSpeedArray.Length == 0) {
                return;
            }
            var stateModel = this.knockBackModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.beHitDir = beHitDir;
            stateModel.knockBackSpeedArray = knockBackSpeedArray;

            stateFlag = stateFlag.AddStateFlag(StateFlag.KnockBack);
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  '{StateFlag.KnockBack}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void AddKnockUp(in KnockUpModel model) {
            var knockUpSpeedArray = model.knockUpSpeedArray;
            if (knockUpSpeedArray == null || knockUpSpeedArray.Length == 0) {
                return;
            }

            var stateModel = this.knockUpModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.knockUpSpeedArray = knockUpSpeedArray;

            stateFlag = stateFlag.AddStateFlag(StateFlag.KnockUp);
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  '{StateFlag.KnockUp}'  \n{stateFlag.ToString_AllFlags()}");
        }

        public void AddDying(int maintainFrame) {
            var stateModel = this.dyingModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.maintainFrame = maintainFrame;

            stateFlag = stateFlag.AddStateFlag(StateFlag.Dying);
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.Idle);

            TDLog.Log($"角色状态机 - 添加  '{StateFlag.Dying}'  \n{stateFlag.ToString_AllFlags()}");
        }

        #endregion

        #region [移除 状态标记]

        public void RemoveIdle() {
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.Idle);
            TDLog.Log($"角色状态机 - 移除  '{StateFlag.Idle}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void RemoveCast() {
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.Cast);
            TDLog.Log($"角色状态机 - 移除  '{StateFlag.Cast}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void RemoveSkillMove() {
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.SkillMove);
            TDLog.Log($"角色状态机 - 移除  '{StateFlag.SkillMove}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void RemoveKnockBack() {
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.KnockBack);
            TDLog.Log($"角色状态机 - 移除  '{StateFlag.KnockBack}'\n{stateFlag.ToString_AllFlags()}");
        }

        public void RemoveKnockUp() {
            stateFlag = stateFlag.RemoveStateFlag(StateFlag.KnockUp);
            TDLog.Log($"角色状态机 - 移除  '{StateFlag.KnockUp}'\n{stateFlag.ToString_AllFlags()}");
        }

        #endregion

        #region [Locomotion 判断]

        /// <summary>
        /// 当前状态 是否可以移动
        /// </summary>
        public bool CanMove() {
            return !stateFlag.Contains(StateFlag.Dying)
                && !stateFlag.Contains(StateFlag.SkillMove)
                && !stateFlag.Contains(StateFlag.KnockBack)
                && !stateFlag.Contains(StateFlag.KnockUp)
                && !stateFlag.Contains(StateFlag.Root)
                && !stateFlag.Contains(StateFlag.Stun);
        }

        /// <summary>
        /// 当前状态 是否可以旋转
        /// </summary>
        public bool CanSkillMove() {
            return !stateFlag.Contains(StateFlag.Dying)
                && !stateFlag.Contains(StateFlag.KnockBack)
                && !stateFlag.Contains(StateFlag.KnockUp)
                && !stateFlag.Contains(StateFlag.Root)
                && !stateFlag.Contains(StateFlag.Stun);
        }

        /// <summary>
        /// 当前状态 是否可以跳跃
        /// </summary>
        public bool CanJump() {
            return !stateFlag.Contains(StateFlag.Dying)
                && !stateFlag.Contains(StateFlag.KnockBack)
                && !stateFlag.Contains(StateFlag.KnockUp)
                && !stateFlag.Contains(StateFlag.Root)
                && !stateFlag.Contains(StateFlag.Stun);
        }

        /// <summary>
        /// 当前状态 是否会下落
        /// </summary>
        public bool CanFall() {
            return !stateFlag.Contains(StateFlag.KnockBack)
                && !stateFlag.Contains(StateFlag.KnockUp)
                && !stateFlag.Contains(StateFlag.Stun);
        }

        #endregion

        #region [施法 判断]

        /// <summary>
        /// 当前状态 是否可以释放 普通技能
        /// </summary>
        public bool CanCast_NormalSkill() {
            return !stateFlag.Contains(StateFlag.Dying)
                && !stateFlag.Contains(StateFlag.Root)
                && !stateFlag.Contains(StateFlag.Stun)
                && !stateFlag.Contains(StateFlag.Silence);
        }

        #endregion

        #region [Idle 判断]

        public bool NeedSetIdle() {
            return stateFlag == 0;
        }

        #endregion

    }
}
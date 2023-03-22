using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleFSMComponent {

        StateFlag stateFlag;
        public StateFlag StateFlag => stateFlag;

        RoleFSMModel_Idle idleModel;
        public RoleFSMModel_Idle IdleModel => idleModel;

        RoleFSMModel_Casting castingModel;
        public RoleFSMModel_Casting CastingModel => castingModel;

        RoleFSMModel_KnockBack knockBackModel;
        public RoleFSMModel_KnockBack KnockBackModel => knockBackModel;

        RoleFSMModel_KnockUp knockUpModel;
        public RoleFSMModel_KnockUp KnockUpModel => knockUpModel;

        RoleFSMModel_Dying dyingModel;
        public RoleFSMModel_Dying DyingModel => dyingModel;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool value) => isExiting = value;

        public RoleFSMComponent() {
            stateFlag = StateFlag.Idle;
            idleModel = new RoleFSMModel_Idle();
            castingModel = new RoleFSMModel_Casting();
            knockBackModel = new RoleFSMModel_KnockBack();
            knockUpModel = new RoleFSMModel_KnockUp();
            dyingModel = new RoleFSMModel_Dying();
        }

        public void Reset() {
            isExiting = false;
            stateFlag = StateFlag.Idle;
            idleModel.Reset();
            castingModel.Reset();
            knockBackModel.Reset();
            dyingModel.Reset();
        }

        #region [Add State Flag]

        public void Add_Idle() {
            var stateModel = idleModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateFlag.AddStateFlag(StateFlag.Idle);
            TDLog.Log($"角色状态机 - 添加  '{StateFlag.Idle}'  \n{stateFlag}");
        }

        public void Add_Cast(int skillTypeID, bool isCombo, Vector2 chosedPoint) {
            var stateModel = castingModel;
            stateModel.Reset();

            stateModel.SetCastingSkillTypeID(skillTypeID);
            stateModel.SetIsCombo(isCombo);
            stateModel.SetChosedPoint(chosedPoint);
            stateModel.SetIsEntering(true);

            stateFlag.AddStateFlag(StateFlag.Cast);
            TDLog.Log($"角色状态机 - 添加  {StateFlag.Cast} {skillTypeID} / 是否连招 {isCombo} / 选择点 {chosedPoint}\n{stateFlag}");
        }

        public void Add_KnockBack(Vector2 beHitDir, in KnockBackPowerModel model) {
            var stateModel = this.knockBackModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.beHitDir = beHitDir;
            stateModel.knockBackSpeedArray = model.knockBackSpeedArray;

            stateFlag.AddStateFlag(StateFlag.KnockBack);
            TDLog.Log($"角色状态机 - 添加  '{StateFlag.KnockBack}'  \n{stateFlag}");
        }

        public void Add_KnockUp(in KnockUpPowerModel model) {
            var stateModel = this.knockUpModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.knockUpSpeedArray = model.knockUpSpeedArray;

            stateFlag.AddStateFlag(StateFlag.KnockUp);
            TDLog.Log($"角色状态机 - 添加  '{StateFlag.KnockUp}'  \n{stateFlag}");
        }

        public void Add_Dying(int maintainFrame) {
            var stateModel = this.dyingModel;
            stateModel.Reset();
            stateModel.SetIsEntering(true);

            stateModel.maintainFrame = maintainFrame;

            stateFlag.AddStateFlag(StateFlag.Dying);
            TDLog.Log($"角色状态机 - 添加  '{StateFlag.Dying}'  \n{stateFlag}");
        }

        #endregion

        #region [Remove State Flag]

        public void Remove_Idle() {
            stateFlag.RemoveStateFlag(StateFlag.Idle);
            TDLog.Log($"角色状态机 - 移除  '{StateFlag.Idle}'  \n{stateFlag}");
        }

        public void Remove_Cast() {
            stateFlag.RemoveStateFlag(StateFlag.Cast);
            TDLog.Log($"角色状态机 - 移除  '{StateFlag.Cast}'  \n{stateFlag}");
        }

        public void Remove_KnockBack() {
            stateFlag.RemoveStateFlag(StateFlag.KnockBack);
            TDLog.Log($"角色状态机 - 移除  '{StateFlag.KnockBack}'  \n{stateFlag}");
        }

        public void Remove_KnockUp() {
            stateFlag.RemoveStateFlag(StateFlag.KnockUp);
            TDLog.Log($"角色状态机 - 移除  '{StateFlag.KnockUp}'  \n{stateFlag}");
        }

        #endregion

    }
}
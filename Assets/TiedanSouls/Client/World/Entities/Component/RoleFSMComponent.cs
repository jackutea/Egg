using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleFSMComponent {

        RoleFSMState state;
        public RoleFSMState State => state;

        RoleFSMModel_Idle idleModel;
        public RoleFSMModel_Idle IdleModel => idleModel;

        RoleFSMModel_Casting castingModel;
        public RoleFSMModel_Casting CastingModel => castingModel;

        RoleFSMModel_BeHit beHitModel;
        public RoleFSMModel_BeHit BeHitModel => beHitModel;

        RoleFSMModel_Dying dyingModel;
        public RoleFSMModel_Dying DyingModel => dyingModel;

        bool isExiting;
        public bool IsExiting => isExiting;
        public void SetIsExiting(bool value) => isExiting = value;

        public RoleFSMComponent() {
            state = RoleFSMState.Idle;
            idleModel = new RoleFSMModel_Idle();
            castingModel = new RoleFSMModel_Casting();
            beHitModel = new RoleFSMModel_BeHit();
            dyingModel = new RoleFSMModel_Dying();
        }

        public void Reset() {
            isExiting = false;
            EnterIdle();
        }

        public void EnterIdle() {
            var stateModel = idleModel;
            stateModel.Reset();

            idleModel.isEntering = true;

            state = RoleFSMState.Idle;
            TDLog.Log("人物状态机切换 - 待机 ");
        }

        public void EnterCasting(int skillTypeID, bool isCombo, Vector2 chosedPoint) {
            var stateModel = castingModel;
            stateModel.Reset();

            stateModel.SetCastingSkillTypeID (skillTypeID);
            stateModel.SetIsCombo(isCombo);
            stateModel.SetChosedPoint(chosedPoint);

            stateModel.SetIsEntering(true);
            state = RoleFSMState.Casting;
            TDLog.Log($"人物状态机切换 - 施法中 {skillTypeID} 连击 {isCombo}");
        }

        public void EnterBeHit(in PhysicsPowerModel physicsPowerModel, int hitFrame, Vector2 beHitDir) {
            var stateModel = beHitModel;
            stateModel.Reset();

            int castingSkillTypeID = -1;
            if (state == RoleFSMState.Casting) castingSkillTypeID = castingModel.CastingSkillTypeID;

            stateModel.isEntering = true;

            stateModel.beHitDir = beHitDir;
            stateModel.castingSkillTypeID = castingSkillTypeID;
            stateModel.knockBackSpeedArray = physicsPowerModel.knockBackSpeedArray;
            stateModel.knockUpSpeedArray = physicsPowerModel.knockUpSpeedArray;

            state = RoleFSMState.BeHit;
            TDLog.Log($"人物状态机切换 - 受击\n受击方向 {beHitDir} / hitFrame {hitFrame} / hitStunFrame {stateModel.hitStunFrame} / 正在释放的技能ID: {castingSkillTypeID}");
        }

        public void EnterDying(int maintainFrame) {
            var stateModel = dyingModel;
            stateModel.Reset();

            dyingModel.SetIsEntering(true);
            dyingModel.maintainFrame = maintainFrame;

            state = RoleFSMState.Dying;
            TDLog.Log("人物状态机切换 - 死亡中 ");
        }

    }
}
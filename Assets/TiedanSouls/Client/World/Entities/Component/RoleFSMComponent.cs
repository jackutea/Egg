using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.World.Entities {

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
            state = RoleFSMState.Idle;
            var stateModel = idleModel;

            stateModel.Reset();
            idleModel.isEntering = true;
            TDLog.Log("人物状态机切换 - 待机 ");
        }

        public void EnterCasting(SkillorModel skillorModel, bool isCombo) {
            state = RoleFSMState.Casting;
            var stateModel = castingModel;

            stateModel.Reset();
            stateModel.castingSkillorTypeID = skillorModel.TypeID;
            stateModel.SetIsCombo(isCombo);
            stateModel.SetIsEntering(true);
            TDLog.Log($"人物状态机切换 - 施放技能TypeID {skillorModel.TypeID} 连击 {isCombo}");
        }

        public void EnterBeHit(Vector2 fromPos, HitPowerModel hitPowerModel) {
            state = RoleFSMState.BeHit;
            var stateModel = beHitModel;

            stateModel.Reset();
            stateModel.fromPos = fromPos;
            stateModel.knockbackForce = hitPowerModel.knockbackForce;
            stateModel.knockbackFrame = hitPowerModel.knockbackFrame;
            stateModel.hitStunFrame = hitPowerModel.hitStunFrame;
            stateModel.curFrame = 0;
            stateModel.isEntering = true;
            TDLog.Log("人物状态机切换 - 受伤 ");
        }

        public void EnterDead(int maintainFrame) {
            state = RoleFSMState.Dying;
            var stateModel = dyingModel;

            stateModel.Reset();
            dyingModel.SetIsEntering(true);
            dyingModel.maintainFrame = maintainFrame;
            TDLog.Log("人物状态机切换 - 死亡中 ");
        }

    }
}
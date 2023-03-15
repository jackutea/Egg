using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World.Domain {

    public class WorldRoleFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldDomain worldDomain;

        public WorldRoleFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void TickFSM(RoleEntity role, float dt) {
            if (role == null) return;

            var fsm = role.FSMCom;
            if (fsm.IsExiting) return;

            Apply_AnyState(role, fsm, dt);

            if (fsm.State == RoleFSMState.Idle) {
                Apply_Idle(role, fsm, dt);
            } else if (fsm.State == RoleFSMState.Casting) {
                Apply_Casting(role, fsm, dt);
            } else if (fsm.State == RoleFSMState.BeHit) {
                Apply_BeHit(role, fsm, dt);
            } else if (fsm.State == RoleFSMState.Dying) {
                Apply_Dead(role, fsm, dt);
            }
        }

        void Apply_AnyState(RoleEntity role, RoleFSMComponent fsm, float dt) {
            if (fsm.State == RoleFSMState.Dying) return;

            var roleDomain = worldDomain.RoleDomain;
            if (roleDomain.CanEnterDying(role)) {
                role.FSMCom.EnterDying(30);
            }
        }

        void Apply_Idle(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.IdleModel;
            if (stateModel.isEntering) {
                stateModel.isEntering = false;
                role.ModCom.Anim_PlayIdle();
            }

            var roleDomain = worldDomain.RoleDomain;

            roleDomain.Move(role);
            roleDomain.Jump(role);
            roleDomain.Falling(role, dt);
            roleDomain.CrossDown(role);

            // Idle状态下 拾取武器
            var inputCom = role.InputCom;
            if (inputCom.HasInput_Basic_Pick) {
                roleDomain.TryPickUpSomethingFromField(role);
            }

            // Idle状态下 释放技能
            if (roleDomain.TryCastSkillByInput(role)) {
                roleDomain.FaceToChoosePoint(role);
            } else {
                roleDomain.FaceToMoveDir(role);
            }
        }

        void Apply_Casting(RoleEntity role, RoleFSMComponent fsm, float dt) {
            if (fsm.State != RoleFSMState.Casting) {
                return;
            }

            var stateModel = fsm.CastingModel;
            var skillTypeID = stateModel.castingSkillTypeID;
            var isCombo = stateModel.IsCombo;
            var skillSlotCom = role.SkillSlotCom;
            _ = skillSlotCom.TryGet(skillTypeID, isCombo, out var castingSkill);

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                castingSkill.SetRootPos(role.GetPos_Logic());
                role.WeaponSlotCom.Weapon.PlayAnim(castingSkill.WeaponAnimName);
            }

            var roleDomain = worldDomain.RoleDomain;

            // 根据 鼠标点击 改变朝向
            roleDomain.FaceToChoosePoint(role);

            // 尝试 技能组合技
            if (roleDomain.TryCastSkillByInput(role)) {
                return;
            }

            // Locomotion
            roleDomain.Move(role);
            roleDomain.Jump(role);
            roleDomain.Falling(role, dt);

            // Next Frame
            if (!castingSkill.TryMoveNext(new Vector2(2, 0))) {
                castingSkill.Reset();
                fsm.EnterIdle();
            }
        }

        void Apply_BeHit(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.BeHitModel;

            if (stateModel.isEntering) {
                stateModel.isEntering = false;

                Vector2 dir = role.GetPos_Logic() - stateModel.fromPos;
                role.MoveCom.KnockBack(dir.x, stateModel.knockbackForce);

                role.ModCom.Anim_Play_BeHit();
                return;
            }

            if (stateModel.curFrame >= stateModel.hitStunFrame && stateModel.curFrame >= stateModel.knockbackFrame) {
                fsm.EnterIdle();
                return;
            }

            stateModel.curFrame += 1;
        }

        void Apply_Dead(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.DyingModel;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);

                role.HudSlotCom.HideHUD();
                role.ModCom.Anim_Play_Dying();
                role.MoveCom.StopHorizontal();
            }

            if (stateModel.maintainFrame <= 0) {
                var roleDomain = worldContext.WorldDomain.RoleDomain;
                roleDomain.Die(role);
                fsm.SetIsExiting(true);
            }

            stateModel.maintainFrame--;
        }

    }
}
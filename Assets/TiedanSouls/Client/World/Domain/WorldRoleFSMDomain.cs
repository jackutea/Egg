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
                // Apply_Casting(role, fsm, dt);
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
                var choosePoint = inputCom.ChoosePoint;
                if (choosePoint != Vector2.zero) {
                    var rolePos = role.GetPos_LogicRoot();
                    var xDiff = choosePoint.x - rolePos.x;
                    var dirX = (sbyte)(xDiff > 0 ? 1 : xDiff == 0 ? 0 : -1);
                    roleDomain.FaceTo(role, dirX);
                }
            } else {
                var x = inputCom.MoveAxis.x;
                var dirX = (sbyte)(x > 0 ? 1 : x == 0 ? 0 : -1);
                roleDomain.FaceTo(role, dirX);
            }
        }

        // void Apply_Casting(RoleEntity role, RoleFSMComponent fsm, float dt) {
        //     if (fsm.State != RoleFSMState.Casting) {
        //         return;
        //     }

        //     var stateModel = fsm.CastingModel;
        //     var skillTypeID = stateModel.castingSkillTypeID;
        //     var isCombo = stateModel.IsCombo;
        //     SkillModel castingSkill;
        //     if (isCombo) {
        //         role.SkillSlotCom.TryGetComboSkill(skillTypeID, out castingSkill);
        //     } else {
        //         role.SkillSlotCom.TryGetOriginalSkillByTypeID(skillTypeID, out castingSkill);
        //     }

        //     if (stateModel.IsEntering) {
        //         stateModel.SetIsEntering(false);


        //         role.WeaponSlotCom.Weapon.PlayAnim(castingSkill.weaponAnimName);
        //     }

        //     if (!castingSkill.TryGetCurrentFrame(out SkillFrameElement curFrame)) {

        //         var damageArbitService = worldContext.DamageArbitService;
        //         damageArbitService.Remove(castingSkill.EntityType, castingSkill.ID);

        //         castingSkill.Reset();

        //         fsm.EnterIdle();

        //         return;
        //     }

        //     var roleDomain = worldDomain.RoleDomain;

        //     // 根据鼠标点击改变朝向
        //     var choosePoint = role.InputCom.ChoosePoint;
        //     if (choosePoint != Vector2.zero) {
        //         var rolePos = role.GetPos_LogicRoot();
        //         var xDiff = choosePoint.x - rolePos.x;
        //         var dirX = (sbyte)(xDiff > 0 ? 1 : xDiff == 0 ? 0 : -1);
        //         roleDomain.SetRoleFaceDirX(role, dirX);
        //     }

        //     // 技能接技能
        //     if (roleDomain.TryCastSkillByInput(role)) {
        //         return;
        //     }

        //     // 角色Locomotion
        //     roleDomain.Move(role);
        //     roleDomain.Jump(role);
        //     roleDomain.Falling(role, dt);

        //     // Dash
        //     if (curFrame.hasDash) {
        //         roleDomain.Dash(role, Vector2.right * role.FaceDirX, curFrame.dashForce);
        //     }

        //     // Next Frame
        //     castingSkill.ActiveNextFrame(role.GetPos_Logic(), role.GetRot_Logic(), role.FaceDirX);
        // }

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
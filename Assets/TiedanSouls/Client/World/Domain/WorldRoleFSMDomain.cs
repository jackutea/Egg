using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client.Domain {

    public class WorldRoleFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldDomain;

        public WorldRoleFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void TickFSM(RoleEntity role, float dt) {
            if (role == null) return;

            var fsm = role.FSMCom;
            if (fsm.IsExiting) return;

            if (fsm.State == RoleFSMState.Idle) {
                Apply_Idle(role, fsm, dt);
            } else if (fsm.State == RoleFSMState.Casting) {
                Apply_Casting(role, fsm, dt);
            } else if (fsm.State == RoleFSMState.BeHit) {
                Apply_BeHit(role, fsm, dt);
            } else if (fsm.State == RoleFSMState.Dying) {
                Apply_Dead(role, fsm, dt);
            }

            Apply_AnyState(role, fsm, dt);
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

            // 拾取武器
            var inputCom = role.InputCom;
            if (inputCom.HasInput_Basic_Pick) {
                roleDomain.TryPickUpSomethingFromField(role);
            }

            // 面向移动方向
            roleDomain.FaceToMoveDir(role);

            // 释放技能
            _ = roleDomain.TryCastSkillByInput(role);

        }

        void Apply_Casting(RoleEntity role, RoleFSMComponent fsm, float dt) {
            if (fsm.State != RoleFSMState.Casting) {
                return;
            }

            var stateModel = fsm.CastingModel;
            var skillTypeID = stateModel.CastingSkillTypeID;
            var isCombo = stateModel.IsCombo;
            var skillSlotCom = role.SkillSlotCom;
            _ = skillSlotCom.TryGet(skillTypeID, isCombo, out var castingSkill);
            var roleDomain = worldDomain.RoleDomain;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                roleDomain.FaceTo_Horizontal(role, stateModel.ChosedPoint);
                role.WeaponSlotCom.Weapon.PlayAnim(castingSkill.WeaponAnimName);
            }

            // 释放技能
            if (roleDomain.TryCastSkillByInput(role)) return;

            // Locomotion
            roleDomain.Move(role);
            roleDomain.Jump(role);
            roleDomain.Falling(role, dt);

            // 技能逻辑迭代
            if (!castingSkill.TryMoveNext(role.GetPos_Logic(), role.GetRot_Logic())) {
                fsm.EnterIdle();
            }
        }

        void Apply_BeHit(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.BeHitModel;

            if (stateModel.isEntering) {
                stateModel.isEntering = false;

                // 受击时技能被打断
                var castingSkillTypeID = stateModel.castingSkillTypeID;
                if (castingSkillTypeID != -1) {
                    var skillSlotCom = role.SkillSlotCom;
                    _ = skillSlotCom.TryGet(castingSkillTypeID, out var castingSkill);
                    castingSkill.Reset();
                }

                role.MoveCom.StopHorizontal();

                role.ModCom.Anim_Play_BeHit();
            }

            stateModel.hitStunFrame--;
            if (stateModel.hitStunFrame <= 0) {
                fsm.EnterIdle();
                return;
            }
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
                var roleDomain = worldContext.RootDomain.RoleDomain;
                roleDomain.Die(role);
                fsm.SetIsExiting(true);
            }

            stateModel.maintainFrame--;
        }

    }
}
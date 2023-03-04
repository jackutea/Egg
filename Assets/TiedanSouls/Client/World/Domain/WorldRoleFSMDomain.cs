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

        public void Tick(RoleEntity role, float dt) {
            // TODO: if else 
            ApplyCasting(role, dt);
            ApplyIdle(role, dt);
            ApplyBeHit(role, dt);
        }

        void ApplyIdle(RoleEntity role, float dt) {
            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.Idle) {
                return;
            }

            var stateModel = fsm.IdleState;
            if (stateModel.isEnter) {
                stateModel.isEnter = false;
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
            if (!roleDomain.TryCastSkillorByInput(role)) {
                var x = inputCom.MoveAxis.x;
                var dirX = (sbyte)(x > 0 ? 1 : x == 0 ? 0 : -1);
                roleDomain.SetRoleFaceDirX(role, dirX);
            } else {
                var choosePoint = inputCom.ChoosePoint;
                if (choosePoint != Vector2.zero) {
                    var rolePos = role.GetPos_LogicRoot();
                    var xDiff = choosePoint.x - rolePos.x;
                    var dirX = (sbyte)(xDiff > 0 ? 1 : xDiff == 0 ? 0 : -1);
                    roleDomain.SetRoleFaceDirX(role, dirX);
                }
            }
        }

        void ApplyCasting(RoleEntity role, float dt) {
            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.Casting) {
                return;
            }

            var stateModel = fsm.CastingState;
            var castingSkillor = stateModel.castingSkillor;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                role.WeaponSlotCom.Weapon.PlayAnim(castingSkillor.weaponAnimName);
            }

            if (!castingSkillor.TryGetCurrentFrame(out SkillorFrameElement curFrame)) {

                var damageArbitService = worldContext.DamageArbitService;
                damageArbitService.Remove(castingSkillor.EntityType, castingSkillor.ID);

                castingSkillor.Reset();

                fsm.EnterIdle();

                return;
            }

            var roleDomain = worldDomain.RoleDomain;
            if (roleDomain.TryCastSkillorByInput(role)) {
                return;
            }

            roleDomain.Move(role);
            roleDomain.Jump(role);
            roleDomain.Falling(role, dt);

            var choosePoint = role.InputCom.ChoosePoint;
            if (choosePoint != Vector2.zero) {
                var rolePos = role.GetPos_LogicRoot();
                var xDiff = choosePoint.x - rolePos.x;
                var dirX = (sbyte)(xDiff > 0 ? 1 : xDiff == 0 ? 0 : -1);
                roleDomain.SetRoleFaceDirX(role, dirX);
            }

            if (curFrame.hasDash) {
                roleDomain.Dash(role, Vector2.right * role.FaceDirX, curFrame.dashForce);
            }

            castingSkillor.ActiveNextFrame(role.GetPos_Logic(), role.GetRot_Logic(), role.FaceDirX);
        }

        void ApplyBeHit(RoleEntity role, float dt) {

            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.BeHit) {
                return;
            }

            var roleDomain = worldDomain.RoleDomain;

            var stateModel = fsm.BeHitState;

            if (stateModel.isEnter) {
                stateModel.isEnter = false;

                Vector2 dir = role.GetPos_Logic() - stateModel.fromPos;
                role.MoveCom.KnockBack(dir.x, stateModel.knockbackForce);

                role.ModCom.Anim_PlayBeHit();
                return;
            }

            if (stateModel.curFrame >= stateModel.hitStunFrame && stateModel.curFrame >= stateModel.knockbackFrame) {
                fsm.EnterIdle();
                return;
            }

            stateModel.curFrame += 1;
        }

    }
}
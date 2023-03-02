using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Entities;
using TiedanSouls.Template;

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
            ApplyBeHurt(role, dt);
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
            roleDomain.CrossDown(role);
            roleDomain.Falling(role, dt);
            _ = roleDomain.TryCancelSkillor(role);

            // - Idle状态下可拾取武器
            var inputCom = role.InputCom;
            if (inputCom.HasInput_Basic_Pick) {
                roleDomain.TryPickUpSomething(role);
            }
        }

        void ApplyCasting(RoleEntity role, float dt) {
            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.Casting) {
                return;
            }

            var stateModel = fsm.CastingState;
            var castingSkillor = stateModel.castingSkillor;

            if (stateModel.isEntering) {
                stateModel.isEntering = false;
                role.MoveCom.StopHorizontal();
                role.WeaponSlotCom.Weapon.PlayAnim(castingSkillor.weaponAnimName);

                return;
            }

            if (!castingSkillor.TryGetCurrentFrame(out SkillorFrameElement curFrame)) {

                var damageArbitService = worldContext.DamageArbitService;
                damageArbitService.Remove(castingSkillor.EntityType, castingSkillor.ID);

                castingSkillor.Reset();

                fsm.EnterIdle();

                return;
            }

            var roleDomain = worldDomain.RoleDomain;

            roleDomain.Falling(role, dt);   // TODO: 离地才下落

            if (curFrame.hasDash) {
                roleDomain.Dash(role, Vector2.right * role.FaceXDir, curFrame.dashForce);
            }

            if (roleDomain.TryCancelSkillor(role)) {
                return;
            }

            castingSkillor.ActiveNextFrame(role.GetRBPos(), role.GetRBAngle(), role.FaceXDir);
        }

        void ApplyBeHurt(RoleEntity role, float dt) {

            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.BeHurt) {
                return;
            }

            var roleDomain = worldDomain.RoleDomain;

            var stateModel = fsm.BeHurtState;

            if (stateModel.isEnter) {
                stateModel.isEnter = false;

                Vector2 dir = role.GetRBPos() - stateModel.fromPos;
                role.MoveCom.KnockBack(dir.x, stateModel.knockbackForce);

                role.ModCom.Anim_PlayBeHurt();
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
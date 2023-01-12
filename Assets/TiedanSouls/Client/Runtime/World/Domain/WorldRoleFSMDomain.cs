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
            ApplyIdle(role, dt);
            ApplyCasting(role, dt);
        }

        void ApplyIdle(RoleEntity role, float dt) {
            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.Idle) {
                return;
            }

            var roleDomain = worldDomain.RoleDomain;
            roleDomain.Move(role);
            roleDomain.Jump(role);
            roleDomain.CrossDown(role);
            roleDomain.Falling(role, dt);
            roleDomain.CastByInput(role);
        }

        void ApplyCasting(RoleEntity role, float dt) {

            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.Casting) {
                return;
            }

            var roleDomain = worldDomain.RoleDomain;

            var stateModel = fsm.CastingState;
            var castingSkillor = stateModel.castingSkillor;

            if (stateModel.isEntering) {
                stateModel.isEntering = false;
                role.WeaponSlotCom.Weapon.PlayAnim(castingSkillor.weaponAnimName);
                return;
            }

            float restTime = stateModel.restTime;
            restTime += dt;

            while (restTime >= stateModel.targetRate) {

                restTime -= stateModel.targetRate;

                SkillorFrameElement frame;
                if (!castingSkillor.TryGetCurrentFrame(out frame)) {

                    // remove damage arbit
                    var damageArbitService = worldContext.DamageArbitService;
                    damageArbitService.Remove(castingSkillor.EntityType, castingSkillor.ID);

                    castingSkillor.Reset();
                    
                    fsm.EnterIdle();
                    
                    // TDLog.Log("END Casting");
                    return;
                }

                // current frame logic
                if (frame.hasDash) {
                    roleDomain.Dash(role, Vector2.right * role.FaceXDir, frame.dashForce);
                }
                roleDomain.Falling(role, dt);

                // next frame
                castingSkillor.ActiveNextFrame(role.transform.position, role.transform.rotation.eulerAngles.z, role.FaceXDir);

            }

            stateModel.restTime = restTime;

        }

    }
}
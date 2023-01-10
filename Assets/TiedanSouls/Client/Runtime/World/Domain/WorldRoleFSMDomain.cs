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
            roleDomain.Falling(role, dt);
            roleDomain.CastByInput(role);
        }

        void ApplyCasting(RoleEntity role, float dt) {
            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.Casting) {
                return;
            }

            var castingSkillor = fsm.CastingState.castingSkillor;
            SkillorFrameElement frame;
            if (!castingSkillor.TryGetCurrentFrame(out frame)) {
                fsm.EnterIdle();
                castingSkillor.Reset();
                TDLog.Log("END Casting");
                return;
            }

            // current frame logic

            // next frame
            castingSkillor.ActiveNextFrame(role.transform.position, role.transform.rotation.eulerAngles.z);

            TDLog.Log("Casting");
        }

    }
}
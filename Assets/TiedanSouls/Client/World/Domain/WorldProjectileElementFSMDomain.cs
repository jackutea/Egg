using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldProjectileElementFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldRootDomain;

        public WorldProjectileElementFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldRootDomain = worldDomain;
        }

        public void TickFSM_AllElements(ProjectileEntity projectile, float dt) {
            var rootElement = projectile.RootElement;
            TickFSM(rootElement, projectile.CurFrame, dt);

            var leafElementArray = projectile.LeafElementArray;
            var len = leafElementArray.Length;
            for (int i = 0; i < len; i++) {
                var leafElement = leafElementArray[i];
                TickFSM(leafElement, projectile.CurFrame, dt);
            }
        }

        void TickFSM(ProjectileElement element, int curFrame, float dt) {
            var fsm = element.FSMCom;
            var state = fsm.State;

            if (state == ProjectileElementFSMState.Deactivated) {
                Apply_Deactivated(element, curFrame, fsm, dt);
            } else if (state == ProjectileElementFSMState.Activated) {
                Apply_Activated(element, curFrame, fsm, dt);
            } else if (state == ProjectileElementFSMState.Dying) {
                Apply_Dying(element, curFrame, fsm, dt);
            }
        }

        void Apply_Deactivated(ProjectileElement element, int curFrame, ProjectileElementFSMComponent fsm, float dt) {
            var triggerStatus = element.GetTriggerStatus(curFrame);
            if (triggerStatus == TriggerStatus.Begin) {
                fsm.Enter_Activated();
                return;
            }
        }

        void Apply_Activated(ProjectileElement element, int curFrame, ProjectileElementFSMComponent fsm, float dt) {
            var moveCom = element.MoveCom;

            var triggerStatus = element.GetTriggerStatus(curFrame);
            if (triggerStatus == TriggerStatus.End) {
                fsm.Enter_Deactivated();
                moveCom.Stop();
                return;
            }

            var model = fsm.ActivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
            }

            // 移动逻辑
            var canMove = element.CanMove(curFrame);
            if (canMove) {
                var speed = element.GetFrameSpeed(curFrame);
                var dir = element.GetFrameDirection(curFrame);
                var velocity = dir * speed;
                moveCom.SetVelocity(velocity);
            }

            var isLastMoveFrame = element.IsJustPassLastMoveFrame(curFrame);
            if (isLastMoveFrame) {
                moveCom.Stop();
            }
        }

        void Apply_Dying(ProjectileElement element, int curFrame, ProjectileElementFSMComponent fsm, float dt) {

        }

    }

}
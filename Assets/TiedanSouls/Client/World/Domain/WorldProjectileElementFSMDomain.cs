using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldBulletFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldRootDomain;

        public WorldBulletFSMDomain() { }

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

        void TickFSM(BulletEntity element, int curFrame, float dt) {
            var fsm = element.FSMCom;
            var state = fsm.State;

            if (state == BulletFSMState.Deactivated) {
                Apply_Deactivated(element, curFrame, fsm, dt);
            } else if (state == BulletFSMState.Activated) {
                Apply_Activated(element, curFrame, fsm, dt);
            } else if (state == BulletFSMState.Dying) {
                Apply_Dying(element, curFrame, fsm, dt);
            }
        }

        void Apply_Deactivated(BulletEntity element, int curFrame, BulletFSMComponent fsm, float dt) {
            var elementTriggerStatus = element.GetElementTriggerStatus(curFrame);
            if (elementTriggerStatus == TriggerStatus.TriggerEnter) {
                // 激活时做一次碰撞盒控制
                var collisionTriggerModel = element.CollisionTriggerModel;
                var collisionTriggerStatus = collisionTriggerModel.GetTriggerStatus(curFrame - collisionTriggerModel.startFrame);
                if (collisionTriggerStatus == TriggerStatus.TriggerEnter) {
                    element.ActivateAllColliderModels();
                } else if (collisionTriggerStatus == TriggerStatus.TriggerExit) {
                    element.DeactivateAllColliderModels();
                }

                fsm.Enter_Activated();
                return;
            }
        }

        void Apply_Activated(BulletEntity element, int curFrame, BulletFSMComponent fsm, float dt) {
            var model = fsm.ActivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
            }

            var moveCom = element.MoveCom;
            var elementTriggerStatus = element.GetElementTriggerStatus(curFrame);
            if (elementTriggerStatus == TriggerStatus.TriggerExit) {
                moveCom.Stop();
                element.DeactivateAllColliderModels();
                fsm.Enter_Deactivated();
                return;
            }

            // 碰撞盒 激活 & 取消激活
            var collisionTriggerModel = element.CollisionTriggerModel;
            var collisionTriggerStatus = collisionTriggerModel.GetTriggerStatus(curFrame - collisionTriggerModel.startFrame);
            if (collisionTriggerStatus == TriggerStatus.TriggerEnter) {
                element.ActivateAllColliderModels();
            } else if (collisionTriggerStatus == TriggerStatus.TriggerExit) {
                element.DeactivateAllColliderModels();
            }

            // 元素 移动
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


            // TODO: 消失逻辑
        }

        void Apply_Dying(BulletEntity element, int curFrame, BulletFSMComponent fsm, float dt) {

        }

    }

}
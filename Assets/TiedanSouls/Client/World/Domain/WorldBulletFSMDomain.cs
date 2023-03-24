using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldBulletFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldBulletFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = worldDomain;
        }

        /// <summary>
        /// 用于更新指定关卡的 子弹 状态机 -1表示所有关卡
        /// </summary>
        public void TickFSM(int curFieldTypeID, float dt) {
            var bulletRepo = worldContext.BulletRepo;
            bulletRepo.Foreach(curFieldTypeID, (bullet) => {
                TickFSM(bullet, dt);
            });
        }

        void TickFSM(BulletEntity bullet, float dt) {
            var fsm = bullet.FSMCom;
            var state = fsm.State;
            if (state == BulletFSMState.Deactivated) {
                Apply_Deactivated(bullet, fsm, dt);
            } else if (state == BulletFSMState.Activated) {
                Apply_Activated(bullet, fsm, dt);
            } else if (state == BulletFSMState.Dying) {
                Apply_Dying(bullet, fsm, dt);
            }
        }

        void Apply_Deactivated(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            // 这里可能的业务需求
            // 比如我放下一个弹道在原地，但是不会立刻触发，而是满足了一定条件才会触发(比如玩家按下某一按键)
        }

        void Apply_Activated(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            var model = fsm.ActivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
            }

            model.curFrame++;

            // 碰撞盒控制
            var collisionTriggerModel = bullet.CollisionTriggerModel;
            var collisionTriggerStatus = collisionTriggerModel.GetTriggerStatus(model.curFrame);
            if (collisionTriggerStatus == TriggerStatus.TriggerEnter) collisionTriggerModel.ActivateAll();
            if (collisionTriggerStatus == TriggerStatus.TriggerExit) collisionTriggerModel.DeactivateAll();

            if (model.curFrame == bullet.TotalFrame - 1) {
                fsm.Enter_Deactivated();
            }
        }

        void Apply_Dying(BulletEntity bullet, BulletFSMComponent fsm, float dt) { }

    }

}
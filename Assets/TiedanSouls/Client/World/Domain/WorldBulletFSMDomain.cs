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
            var removeList = bulletRepo.ForeachAndGetRemoveList(curFieldTypeID, (bullet) => {
                TickFSM(bullet, dt);
            });

            removeList.ForEach((entityID) => {
                bulletRepo.Remove(entityID);
            });
        }

        void TickFSM(BulletEntity bullet, float dt) {
            var fsm = bullet.FSMCom;
            var state = fsm.State;
            if (state == BulletFSMState.Deactivated) {
                Apply_Deactivated(bullet, fsm, dt);
            } else if (state == BulletFSMState.Activated) {
                Apply_Activated(bullet, fsm, dt);
            } else if (state == BulletFSMState.TearDown) {
                Apply_TearDown(bullet, fsm, dt);
            }

            Apply_Any(bullet, fsm, dt);
        }

        void Apply_Any(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            if (fsm.State == BulletFSMState.TearDown) return;

            if (bullet.ExtraPenetrateCount < 0) fsm.Enter_TearDown(0);
        }

        void Apply_Deactivated(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            // 这里可能的业务需求
            // 比如我放下一个弹道在原地，但是不会立刻触发，而是满足了一定条件才会触发(比如玩家按下某一按键)
            var model = fsm.DeactivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
                bullet.Deactivate();
            }
        }

        void Apply_Activated(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            var model = fsm.ActivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
            }

            var curFrame = model.curFrame;
            curFrame++;

            // 碰撞盒控制
            var collisionTriggerModel = bullet.CollisionTriggerModel;
            var collisionTriggerStatus = collisionTriggerModel.GetTriggerStatus(curFrame);

            if (collisionTriggerStatus == TriggerStatus.TriggerEnter) collisionTriggerModel.ActivateAll();
            if (collisionTriggerStatus == TriggerStatus.TriggerExit) collisionTriggerModel.DeactivateAll();

            // 移动逻辑
            var moveCom = bullet.MoveCom;
            if (bullet.TryGetMoveSpeed(curFrame, out var speed)) {
                _ = bullet.TryGetMoveDir(curFrame, out var moveDir);
                var velocity = moveDir * speed;
                moveCom.SetVelocity(velocity);
            } else if (bullet.IsJustPassLastMoveFrame(curFrame)) {
                moveCom.Stop();
            }

            if (curFrame == bullet.MoveTotalFrame - 1) {
                moveCom.Stop();
                fsm.Enter_TearDown(0);
                // TODO: 业务逻辑：子弹到达终点后，不一定立马消失，有可能进入未激活状态，等待 某一事件 or 固定事件 继续激活
            }

            model.curFrame = curFrame;
        }

        void Apply_TearDown(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            var model = fsm.TearDownModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
            }

            var maintainFrame = model.maintainFrame;
            maintainFrame--;

            if (maintainFrame <= 0) {
                bullet.TearDown();
                fsm.SetIsExiting(true);
                TDLog.Log("子弹 TearDown - 设置状态机退出");
                return;
            }

            model.maintainFrame = maintainFrame;
        }

    }

}
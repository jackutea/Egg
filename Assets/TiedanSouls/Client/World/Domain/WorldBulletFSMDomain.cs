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

            bulletRepo.ReclycleToPool_NoneState();
        }

        void TickFSM(BulletEntity bullet, float dt) {
            var fsm = bullet.FSMCom;
            var state = fsm.State;
            if (state == BulletFSMState.Deactivated) {
                Tick_Deactivated(bullet, fsm, dt);
            } else if (state == BulletFSMState.Activated) {
                Tick_Activated(bullet, fsm, dt);
            } else if (state == BulletFSMState.TearDown) {
                Tick_TearDown(bullet, fsm, dt);
            }

            Tick_Any(bullet, fsm, dt);
        }

        void Tick_Any(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            if (fsm.State == BulletFSMState.None) return;

            // TearDown Check
            if (fsm.State != BulletFSMState.TearDown) {
                if (bullet.ExtraPenetrateCount < 0) fsm.Enter_TearDown(0);
            }
        }

        void Tick_Deactivated(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            var model = fsm.DeactivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
                var moveCom = bullet.MoveCom;
                moveCom.Stop();
                bullet.Deactivate();
            }
        }

        void Tick_Activated(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            var model = fsm.ActivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
                bullet.Activate();
            }

            model.curFrame++;

            // 如果是追踪类型，需要设置追踪目标
            if (bullet.TrajectoryType == TrajectoryType.Track) {
                bullet.entityTrackModel.target = default;
                this.rootDomain.TrySetEntityTrackTarget(ref bullet.entityTrackModel, bullet.IDCom.ToArgs());
            }

            // 碰撞盒控制
            var collisionTriggerModel = bullet.CollisionTriggerModel;
            var collisionTriggerStatus = collisionTriggerModel.GetTriggerStatus(model.curFrame);
            if (collisionTriggerStatus == TriggerStatus.TriggerEnter) {
                collisionTriggerModel.ActivateAll();
            } else if (collisionTriggerStatus == TriggerStatus.TriggerExit) {
                collisionTriggerModel.DeactivateAll();
            } else if (collisionTriggerStatus == TriggerStatus.None) {
                collisionTriggerModel.DeactivateAll();
            }

            // TODO : 不根据碰撞器的生命周期来控制子弹的生命周期，添加子弹自己的生命周期
            if (model.curFrame > bullet.CollisionTriggerModel.totalFrame) {
                fsm.Enter_TearDown(0);
                return;
            }

            // 移动逻辑(根据轨迹类型)
            var moveCom = bullet.MoveCom;
            var trajectoryType = bullet.TrajectoryType;
            var bulletDomain = this.rootDomain.BulletDomain;
            if (trajectoryType == TrajectoryType.Track) {
                bulletDomain.MoveToTrackingTarget(bullet);
            } else if (trajectoryType == TrajectoryType.Curve) {
                bulletDomain.MoveStraight(bullet);
            } else {
                TDLog.Error($"未处理的移动轨迹类型 {trajectoryType}");
            }
        }

        void Tick_TearDown(BulletEntity bullet, BulletFSMComponent fsm, float dt) {
            var model = fsm.TearDownModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
            }

            var maintainFrame = model.maintainFrame;
            maintainFrame--;

            if (maintainFrame <= 0) {
                fsm.Enter_None();
                return;
            }

            model.maintainFrame = maintainFrame;
        }

    }

}
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
            var fsmCom = bullet.FSMCom;
            var state = fsmCom.State;
            if (state == BulletFSMState.Deactivated) {
                Tick_Deactivated(bullet, fsmCom, dt);
            } else if (state == BulletFSMState.Activated) {
                Tick_Activated(bullet, fsmCom, dt);
            } else if (state == BulletFSMState.Dying) {
                Tick_TearDown(bullet, fsmCom, dt);
            }

            Tick_Any(bullet, fsmCom, dt);
        }

        void Tick_Any(BulletEntity bullet, BulletFSMComponent fsmCom, float dt) {
            if (fsmCom.State == BulletFSMState.None) return;

            // TearDown Check
            if (fsmCom.State != BulletFSMState.Dying) {
                if (bullet.ExtraPenetrateCount < 0) fsmCom.Enter_Dying(0);
            }
        }

        void Tick_Deactivated(BulletEntity bullet, BulletFSMComponent fsmCom, float dt) {
            var model = fsmCom.DeactivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
                var moveCom = bullet.MoveCom;
                moveCom.Stop();
                bullet.Deactivate();
            }
        }

        void Tick_Activated(BulletEntity bullet, BulletFSMComponent fsmCom, float dt) {
            var model = fsmCom.ActivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
                bullet.Activate();
            }

            model.curFrame++;

            // 碰撞盒控制
            var collisionTriggerModel = bullet.CollisionTriggerModel;
            var collisionTriggerStatus = collisionTriggerModel.GetTriggerState(model.curFrame);
            if (collisionTriggerStatus == TriggerState.Enter) {
                collisionTriggerModel.ActivateAll();
            } else if (collisionTriggerStatus == TriggerState.Exit) {
                collisionTriggerModel.DeactivateAll();
            } else if (collisionTriggerStatus == TriggerState.None) {
                collisionTriggerModel.DeactivateAll();
            }

            if (model.curFrame >= bullet.MaintainFrame) {
                fsmCom.Enter_Dying(0);
                return;
            }

            // 如果是追踪类型，需要设置追踪目标
            if (bullet.TrajectoryType == TrajectoryType.Track) {
                bullet.entityTrackModel.target = default;
                this.rootDomain.TrySetEntityTrackTarget(ref bullet.entityTrackModel, bullet.IDCom.ToArgs());
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

        void Tick_TearDown(BulletEntity bullet, BulletFSMComponent fsmCom, float dt) {
            var model = fsmCom.TearDownModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
            }

            var maintainFrame = model.maintainFrame;
            maintainFrame--;

            if (maintainFrame <= 0) {
                fsmCom.Enter_None();
                return;
            }

            model.maintainFrame = maintainFrame;
        }

        public void Enter_Dying(BulletEntity bullet) {
            var fsmCom = bullet.FSMCom;
            fsmCom.Enter_Dying(0);
        }

    }

}
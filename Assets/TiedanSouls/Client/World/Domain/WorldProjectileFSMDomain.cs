using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldProjectileFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldRootDomain;

        public WorldProjectileFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldRootDomain = worldDomain;
        }

        /// <summary>
        /// 用于更新指定关卡的弹道状态机 -1表示所有关卡
        /// </summary>
        public void TickFSM(int curFieldTypeID, float dt) {
            var projectileRepo = worldContext.ProjectileRepo;
            projectileRepo.Foreach(curFieldTypeID, (projectile) => {
                TickFSM(projectile, dt);
            });
        }

        void TickFSM(ProjectileEntity projectile, float dt) {
            var fsm = projectile.FSMCom;
            var state = fsm.State;
            if (state == ProjectileFSMState.Deactivated) {
                Apply_Deactivated(projectile, fsm, dt);
            } else if (state == ProjectileFSMState.Activated) {
                Apply_Activated(projectile, fsm, dt);
            } else if (state == ProjectileFSMState.Dying) {
                Apply_Dying(projectile, fsm, dt);
            }
        }

        void Apply_Deactivated(ProjectileEntity projectile, ProjectileFSMComponent fsm, float dt) {
            // 这里可能的业务需求
            // 比如我放下一个弹道在原地，但是不会立刻触发，而是满足了一定条件才会触发(比如玩家按下某一按键)
        }

        void Apply_Activated(ProjectileEntity projectile, ProjectileFSMComponent fsm, float dt) {
            var stateModel = fsm.ActivatedModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
            }

            projectile.AddFrame();

            var curFrame = projectile.CurFrame;

            int aliveCount = 0;
            var bulletRepo = worldContext.BulletRepo;
            var projectileBulletModelArray = projectile.ProjectileBulletModelArray;
            var bulletIDArray = projectile.BulletIDArray;
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                if (!bulletRepo.TryGet(bulletIDArray[i], out var bullet)) {
                    continue;
                }

                var bulletFSM = bullet.FSMCom;

                var startFrame = projectileBulletModelArray[i].startFrame;
                if (curFrame == startFrame) {
                    bulletFSM.Enter_Activated();
                }

                if (bulletFSM.State != BulletFSMState.None && bulletFSM.State != BulletFSMState.Dying) {
                    aliveCount++;
                }

            }

            if (aliveCount == 0) {
                fsm.Enter_Dying(0);
            }
        }

        void Apply_Dying(ProjectileEntity projectile, ProjectileFSMComponent fsm, float dt) {
            var model = fsm.DyingModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);
            }

            fsm.Enter_None();
        }

    }

}
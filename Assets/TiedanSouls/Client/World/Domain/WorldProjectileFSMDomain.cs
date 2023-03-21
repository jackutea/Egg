using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;
using UnityEngine;

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

        void Apply_Activated(ProjectileEntity projectile, ProjectileFSMComponent fsm, float dt) {
            var model = fsm.ActivatedModel;
            if (model.IsEntering) {
                model.SetIsEntering(false);

                // 首次激活
            }

            projectile.MoveNext(dt);
        }

        void Apply_Deactivated(ProjectileEntity projectile, ProjectileFSMComponent fsm, float dt) {

        }

        void Apply_Dying(ProjectileEntity projectile, ProjectileFSMComponent fsm, float dt) {

        }

    }

}
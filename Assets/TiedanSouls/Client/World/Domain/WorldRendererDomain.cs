using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;

namespace TiedanSouls.Client.Domain {

    public class WorldRendererDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldDomain;

        public WorldRendererDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void Tick(float dt) {
            var repo = worldContext.RoleRepo;
            repo.Foreach_All((role) => {
                role.Renderer_Easing(dt);
                role.HudSlotCom.HpBarHUD.Tick(dt);
            });

            var projectileRepo = worldContext.ProjectileRepo;
            projectileRepo.Foreach(-1, (projectile) => {
                projectile.Renderer_Easing(dt);
            });
        }

    }
}
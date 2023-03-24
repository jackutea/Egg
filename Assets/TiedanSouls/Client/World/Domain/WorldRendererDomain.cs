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
            // 角色
            var roleRepo = worldContext.RoleRepo;
            roleRepo.Foreach_All((role) => {
                role.Renderer_Easing(dt);
                role.HudSlotCom.HpBarHUD.Tick(dt);
            });

            // 子弹
            var bulletRepo = worldContext.BulletRepo;
            bulletRepo.Foreach(-1, (bullet) => {
                bullet.Renderer_Easing(dt);
            });
        }

    }
}
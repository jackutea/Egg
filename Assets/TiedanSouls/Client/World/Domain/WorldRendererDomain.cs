using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;

namespace TiedanSouls.Client.Domain {

    public class WorldRendererDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldRendererDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public void Tick(float dt) {
            // 角色
            var roleDomain = worldContext.RootDomain.RoleDomain;
            var roleRepo = worldContext.RoleRepo;
            roleRepo.Foreach_All((role) => {
                roleDomain.Renderer_Easing(role, dt);

                var attributeCom = role.AttributeCom;
                var hudSlotCom = role.HudSlotCom;
                var hpBar = hudSlotCom.HPBarHUD;
                hpBar.SetGP(attributeCom.GP);
                hpBar.SetHP(attributeCom.HP);
                hpBar.SetHPMax(attributeCom.HPMax);
                role.HudSlotCom.HPBarHUD.Tick(dt);
            });

            // 子弹
            var bulletRepo = worldContext.BulletRepo;
            bulletRepo.Foreach(-1, (bullet) => {
                bullet.EasingRenderer(dt);
            });
        }

    }
}
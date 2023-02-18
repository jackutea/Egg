using System.Threading.Tasks;

namespace TiedanSouls.Asset {

    public class AssetCore {

        ContainerModAssets containerModAssets;
        public ContainerModAssets ContainerModAssets => containerModAssets;

        FieldModAssets fieldModAssets;
        public FieldModAssets FieldModAssets => fieldModAssets;

        SpriteAssets spriteAssets;
        public SpriteAssets SpriteAssets => spriteAssets;

        WeaponModAssets weaponModAssets;
        public WeaponModAssets WeaponModAssets => weaponModAssets;

        RoleModAssets roleModAssets;
        public RoleModAssets RoleModAssets => roleModAssets;

        HUDAssets hudAssets;
        public HUDAssets HUDAssets => hudAssets;

        public AssetCore() {
            containerModAssets = new ContainerModAssets();
            fieldModAssets = new FieldModAssets();
            spriteAssets = new SpriteAssets();
            weaponModAssets = new WeaponModAssets();
            roleModAssets = new RoleModAssets();
            hudAssets = new HUDAssets();
        }

        public async Task Init() {
            await containerModAssets.LoadAll();
            await fieldModAssets.LoadAll();
            await spriteAssets.LoadAll();
            await weaponModAssets.LoadAll();
            await roleModAssets.LoadAll();
            await hudAssets.LoadAll();
        }

    }
}
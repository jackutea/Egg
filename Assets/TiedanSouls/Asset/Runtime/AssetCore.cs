using System.Threading.Tasks;

namespace TiedanSouls.Asset {

    public class AssetCore {

        WorldAssets worldAssets;
        public WorldAssets WorldAssets => worldAssets;

        SpriteAssets spriteAssets;
        public SpriteAssets SpriteAssets => spriteAssets;

        WeaponModAssets weaponModAssets;
        public WeaponModAssets WeaponModAssets => weaponModAssets;

        RoleModAssets roleModAssets;
        public RoleModAssets RoleModAssets => roleModAssets;

        HUDAssets hudAssets;
        public HUDAssets HUDAssets =>hudAssets;
        
        public AssetCore() {
            worldAssets = new WorldAssets();
            spriteAssets = new SpriteAssets();
            weaponModAssets = new WeaponModAssets();
            roleModAssets = new RoleModAssets();
            hudAssets = new HUDAssets();
        }

        public async Task Init() {
            await worldAssets.LoadAll();
            await spriteAssets.LoadAll();
            await weaponModAssets.LoadAll();
            await roleModAssets.LoadAll();
            await hudAssets.LoadAll();
        }

    }
}
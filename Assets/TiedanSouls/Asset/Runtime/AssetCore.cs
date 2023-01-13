using System.Threading.Tasks;

namespace TiedanSouls.Asset {

    public class AssetCore {

        WorldAssets worldAssets;
        public WorldAssets WorldAssets => worldAssets;

        SpriteAssets spriteAssets;
        public SpriteAssets SpriteAssets => spriteAssets;

        WeaponAssets weaponAssets;
        public WeaponAssets WeaponAssets => weaponAssets;

        HUDAssets hudAssets;
        public HUDAssets HUDAssets =>hudAssets;
        
        public AssetCore() {
            worldAssets = new WorldAssets();
            spriteAssets = new SpriteAssets();
            weaponAssets = new WeaponAssets();
            hudAssets = new HUDAssets();
        }

        public async Task Init() {
            await worldAssets.LoadAll();
            await spriteAssets.LoadAll();
            await weaponAssets.LoadAll();
            await hudAssets.LoadAll();
        }

    }
}
using System.Threading.Tasks;

namespace TiedanSouls.Asset {

    public class AssetCore {

        WorldAssets worldAssets;
        public WorldAssets WorldAssets => worldAssets;

        SpriteAssets spriteAssets;
        public SpriteAssets SpriteAssets => spriteAssets;

        public AssetCore() {
            worldAssets = new WorldAssets();
            spriteAssets = new SpriteAssets();
        }

        public async Task Init() {
            await worldAssets.LoadAll();
            await spriteAssets.LoadAll();
        }

    }
}
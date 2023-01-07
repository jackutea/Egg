namespace TiedanSouls.Asset.Facades {

    public class AssetContext {

        WorldAssets worldAssets;
        public WorldAssets WorldAssets => worldAssets;

        public AssetContext() {
            worldAssets = new WorldAssets();
        }
        
    }
}
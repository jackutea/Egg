using System.Threading.Tasks;
using TiedanSouls.Asset.Facades;

namespace TiedanSouls.Asset {

    public class AssetCore {

        AssetContext assetContext;

        AssetGetterAPI getterAPI;
        public IAssetGetterAPI Getter => getterAPI;

        public AssetCore() {
            
            assetContext = new AssetContext();

            getterAPI = new AssetGetterAPI();
            getterAPI.Inject(assetContext);

        }

        public async Task Init() {
            await assetContext.WorldAssets.LoadAll();
        }

    }
}
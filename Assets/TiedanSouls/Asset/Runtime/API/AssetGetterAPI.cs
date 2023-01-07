using UnityEngine;
using TiedanSouls.Asset.Facades;

namespace TiedanSouls.Asset {

    public class AssetGetterAPI : IAssetGetterAPI {

        AssetContext assetContext;

        public AssetGetterAPI() { }

        public void Inject(AssetContext assetContext) {
            this.assetContext = assetContext;
        }

        public bool TryGetWorldAsset(string assetName, out GameObject go) {
            return assetContext.WorldAssets.TryGet(assetName, out go);
        }

    }

}
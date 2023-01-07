using UnityEngine;

namespace TiedanSouls.Asset {

    public interface IAssetGetterAPI {

        bool TryGetWorldAsset(string assetName, out GameObject go);

    }

}
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TiedanSouls.Asset {

    public class HUDAssets{
        
        Dictionary<string, GameObject> all;

        public HUDAssets() {
            all = new Dictionary<string, GameObject>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetsLabelCollection.HUD;
            var list = await Addressables.LoadAssetsAsync<GameObject>(label, null).Task;
            foreach (var go in list) {
                all.Add(go.name, go);
            }
        }

        public bool TryGet(string name, out GameObject go) {
            return all.TryGetValue(name, out go);
        }

    }
}

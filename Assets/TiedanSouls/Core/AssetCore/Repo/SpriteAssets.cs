using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Asset {

    public class SpriteAsset {

        Dictionary<string, Sprite> all;

        public SpriteAsset() {
            all = new Dictionary<string, Sprite>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetLabelCollection.SPRITE;
            var list = await Addressables.LoadAssetsAsync<Sprite>(label, null).Task;
            foreach (var item in list) {
                all.Add(item.name, item);
            }
        }

        public bool TryGet(string name, out Sprite sprite) {
            return all.TryGetValue(name, out sprite);
        }

    }
}
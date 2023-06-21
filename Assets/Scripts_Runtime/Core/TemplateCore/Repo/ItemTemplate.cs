using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    public class ItemTemplate {

        Dictionary<int, ItemTM> all;

        public ItemTemplate() {
            all = new Dictionary<int, ItemTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetLabelCollection.SO_ITEM;
            var list = await Addressables.LoadAssetsAsync<ItemSO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int typeID, out ItemTM tm) {
            return all.TryGetValue(typeID, out tm);
        }
    }

}
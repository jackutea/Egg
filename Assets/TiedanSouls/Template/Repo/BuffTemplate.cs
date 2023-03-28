using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    public class BuffTemplate {

        Dictionary<int, BuffTM> all;

        public BuffTemplate() {
            all = new Dictionary<int, BuffTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetLabelCollection.SO_BUFF;
            var list = await Addressables.LoadAssetsAsync<BuffSO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int typeID, out BuffTM tm) {
            return all.TryGetValue(typeID, out tm);
        }
    }

}
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    public class AITemplate {

        Dictionary<int, AITM> all;

        public AITemplate() {
            all = new Dictionary<int, AITM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetLabelCollection.SO_AI;
            var list = await Addressables.LoadAssetsAsync<AISO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int TypeID, out AITM tm) {
            return all.TryGetValue(TypeID, out tm);
        }
    }

}

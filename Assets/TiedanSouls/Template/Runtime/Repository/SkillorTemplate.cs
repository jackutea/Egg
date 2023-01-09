using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace TiedanSouls.Template {

    public class SkillorTemplate {

        Dictionary<int, SkillorTM> all;

        public SkillorTemplate() {
            all = new Dictionary<int, SkillorTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetsLabelCollection.TM_SKILLOR;
            var list = await Addressables.LoadAssetsAsync<SkillorSo>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int typeID, out SkillorTM tm) {
            return all.TryGetValue(typeID, out tm);
        }

    }

}
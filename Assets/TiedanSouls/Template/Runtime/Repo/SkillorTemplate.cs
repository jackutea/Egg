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
            label.labelString = AssetsLabelCollection.SO_SKILLOR;
            var list = await Addressables.LoadAssetsAsync<SkillorSO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int typeID, out SkillorTM tm) {
            if (!all.TryGetValue(typeID, out tm)) {
                TDLog.Warning($"{nameof(SkillorTemplate)}: {typeID} not found!");
                return false;
            }

            return true;
        }

    }

}
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    public class FieldTemplate {

        Dictionary<int, FieldTM> all;

        public FieldTemplate() {
            all = new Dictionary<int, FieldTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetLabelCollection.SO_Field;
            var list = await Addressables.LoadAssetsAsync<FieldSO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int typeID, out FieldTM tm) {
            return all.TryGetValue(typeID, out tm);
        }
    }

}
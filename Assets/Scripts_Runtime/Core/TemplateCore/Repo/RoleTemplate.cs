using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    public class RoleTemplate {

        Dictionary<int, RoleTM> all;

        public RoleTemplate() {
            all = new Dictionary<int, RoleTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetLabelCollection.SO_ROLE;
            var list = await Addressables.LoadAssetsAsync<RoleSO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int typeID, out RoleTM tm) {
            return all.TryGetValue(typeID, out tm);
        }
    }

}
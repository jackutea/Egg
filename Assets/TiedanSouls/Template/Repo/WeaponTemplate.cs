using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    public class WeaponTemplate {

        Dictionary<int, WeaponTM> all;

        public WeaponTemplate() {
            all = new Dictionary<int, WeaponTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetsLabelCollection.SO_WEAPON;
            var list = await Addressables.LoadAssetsAsync<WeaponSO>(label, null).Task;
            foreach (var so in list) {
                var tm = so.weaponTM;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int typeID, out WeaponTM tm) {
            return all.TryGetValue(typeID, out tm);
        }

    }

}
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    public class BulletTemplate {

        Dictionary<int, BulletTM> all;

        public BulletTemplate() {
            all = new Dictionary<int, BulletTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetLabelCollection.SO_BULLET;
            var list = await Addressables.LoadAssetsAsync<BulletSO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int typeID, out BulletTM tm) {
            return all.TryGetValue(typeID, out tm);
        }
    }

}
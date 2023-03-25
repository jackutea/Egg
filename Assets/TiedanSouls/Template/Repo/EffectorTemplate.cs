using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    public class EffectorTemplate {

        Dictionary<int, EffectorTM> all;

        public EffectorTemplate() {
            all = new Dictionary<int, EffectorTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetsLabelCollection.SO_EFFECTOR;
            var list = await Addressables.LoadAssetsAsync<EffectorSO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
                Debug.Log($"加载效果器模板: 类型ID {tm.typeID}");
            }
        }

        public bool TryGet(int TypeID, out EffectorTM tm) {
            return all.TryGetValue(TypeID, out tm);
        }
    }

}

using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    public class EffectorTemplate {

        Dictionary<int, RoleEffectorTM> all;

        public EffectorTemplate() {
            all = new Dictionary<int, RoleEffectorTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetLabelCollection.SO_EFFECTOR;
            var list = await Addressables.LoadAssetsAsync<RoleEffectorSO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
                Debug.Log($"加载效果器模板: 类型ID {tm.typeID}");
            }
        }

        public bool TryGet(int TypeID, out RoleEffectorTM tm) {
            return all.TryGetValue(TypeID, out tm);
        }
    }

}

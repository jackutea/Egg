using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    public class ProjectileTemplate {

        Dictionary<int, ProjectileTM> all;

        public ProjectileTemplate() {
            all = new Dictionary<int, ProjectileTM>();
        }

        public async Task LoadAll() {
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetsLabelCollection.SO_PROJECTILE;
            var list = await Addressables.LoadAssetsAsync<ProjectileSO>(label, null).Task;
            foreach (var item in list) {
                var tm = item.tm;
                all.Add(tm.typeID, tm);
            }
        }

        public bool TryGet(int typeID, out ProjectileTM tm) {
            return all.TryGetValue(typeID, out tm);
        }
    }

}
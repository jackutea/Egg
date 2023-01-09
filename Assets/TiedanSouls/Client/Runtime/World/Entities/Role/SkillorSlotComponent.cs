using System.Collections.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillorSlotComponent {

        Dictionary<int, SkillorModel> all;

        public SkillorSlotComponent() {
            all = new Dictionary<int, SkillorModel>();
        }

        public void Add(SkillorModel model) {
            all.Add(model.TypeID, model);
        }

        public void Remove(int typeID) {
            all.Remove(typeID);
        }

        public bool TryGet(int typeID, out SkillorModel model) {
            return all.TryGetValue(typeID, out model);
        }

        public IEnumerable<SkillorModel> GetAll() {
            return all.Values;
        }

    }

}
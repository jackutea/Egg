using System.Collections.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillorSlotComponent {

        Dictionary<int, SkillorModel> all;

        Dictionary<SkillorType, SkillorModel> allByType;

        public SkillorSlotComponent() {
            all = new Dictionary<int, SkillorModel>();
            allByType = new Dictionary<SkillorType, SkillorModel>();
        }

        public void Add(SkillorModel model) {
            all.Add(model.TypeID, model);
            if (model.SkillorType != SkillorType.Normal) {
                allByType.Add(model.SkillorType, model);
            }
        }

        public void Remove(int typeID) {
            bool has = all.TryGetValue(typeID, out var model);
            if (has) {
                all.Remove(typeID);
                allByType.Remove(model.SkillorType);
            }
        }

        public bool TryGet(int typeID, out SkillorModel model) {
            return all.TryGetValue(typeID, out model);
        }

        public bool TryGetByType(SkillorType type, out SkillorModel model) {
            return allByType.TryGetValue(type, out model);
        }

        public IEnumerable<SkillorModel> GetAll() {
            return all.Values;
        }

    }

}
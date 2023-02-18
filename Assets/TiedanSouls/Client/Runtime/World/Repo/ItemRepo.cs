using System.Collections.Generic;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World {

    public class ItemRepo {

        Dictionary<int, ItemEntity> all;

        public ItemRepo() {
            all = new Dictionary<int, ItemEntity>();
        }

        public bool TryGet(int itemID, out ItemEntity item) {
            return all.TryGetValue(itemID, out item);
        }

        public void Add(ItemEntity item) {
            all.Add(item.ID, item);
        }

        public void Remove(ItemEntity item) {
            all.Remove(item.ID);
        }

        public IEnumerable<ItemEntity> GetAll() {
            return all.Values;
        }

    }
}
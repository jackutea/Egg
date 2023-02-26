using System.Collections.Generic;
using TiedanSouls.World.Entities;
using UnityEngine;

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

        public bool TryGetOneItem(Vector2 pos, float disRange, out ItemEntity item) {
            item = null;
            var e = all.Values.GetEnumerator();
            float minDis = disRange;
            while (e.MoveNext()) {
                var curItem = e.Current;
                var dis = Vector2.Distance(pos, curItem.transform.position);
                if (dis < minDis) {
                    item = curItem;
                    minDis = dis;
                }
            }

            return item != null;
        }


    }
}
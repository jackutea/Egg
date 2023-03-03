using System.Collections.Generic;
using TiedanSouls.World.Entities;
using UnityEngine;

namespace TiedanSouls.World {

    public class ItemRepo {

        Dictionary<int, List<ItemEntity>> all_dic;
        List<ItemEntity> all_list;

        public ItemRepo() {
            all_dic = new Dictionary<int, List<ItemEntity>>();
            all_list = new List<ItemEntity>();
        }

        public bool TryGet(int itemEntityD, out ItemEntity item) {
            item = null;
            var len = all_list.Count;
            for (int i = 0; i < len; i++) {
                var curItem = all_list[i];
                if (curItem.EntityD == itemEntityD) {
                    item = curItem;
                    return true;
                }
            }
            return false;
        }

        public void Add(ItemEntity item, int fromFieldTypeID) {
            if (!all_dic.TryGetValue(fromFieldTypeID, out var list)) {
                list = new List<ItemEntity>();
                all_dic.Add(fromFieldTypeID, list);
            }

            list.Add(item);
        }

        public void Remove(ItemEntity item) {
            all_dic.Remove(item.EntityD);
            all_list.Remove(item);
        }

        public bool TryGetOneItem(Vector2 pos, float disRange, out ItemEntity item) {
            item = null;
            var len = all_list.Count;
            for (int i = 0; i < len; i++) {
                var curItem = all_list[i];
                var curPos = curItem.transform.position;
                var dis = Vector2.Distance(pos, curPos);
                if (dis < disRange) {
                    item = curItem;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetOneItemFromField(int fromFieldTypeID, Vector2 pos, float disRange, out ItemEntity item) {
            item = null;
            if (!all_dic.TryGetValue(fromFieldTypeID, out var list)) {
                return false;
            }

            var len = list.Count;
            for (int i = 0; i < len; i++) {
                var curItem = list[i];
                var curPos = curItem.transform.position;
                var dis = Vector2.Distance(pos, curPos);
                if (dis < disRange) {
                    item = curItem;
                    return true;
                }
            }

            return false;
        }

        public void HideAllItemsInField(int fromFieldTypeID) {
            if (!all_dic.TryGetValue(fromFieldTypeID, out var list)) {
                return;
            }

            var len = list.Count;
            for (int i = 0; i < len; i++) {
                var curItem = list[i];
                curItem.Hide();
            }
        }

        public void ShowAllItemsInField(int fromFieldTypeID) {
            if (!all_dic.TryGetValue(fromFieldTypeID, out var list)) {
                return;
            }

            var len = list.Count;
            for (int i = 0; i < len; i++) {
                var curItem = list[i];
                curItem.Show();
            }
        }

    }
}
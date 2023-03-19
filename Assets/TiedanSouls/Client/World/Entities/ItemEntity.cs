using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class ItemEntity : MonoBehaviour {

        int entityID;
        public int EntityD => entityID;
        public void SetEntityD(int v) => entityID = v;

        int typeID;
        public int TypeID => typeID;
        public void SetTypeID(int v) => typeID = v;

        ItemType itemType;
        public ItemType ItemType => itemType;
        public void SetItemType(ItemType v) => itemType = v;

        int typeIDForPickUp;
        public int TypeIDForPickUp => typeIDForPickUp;
        public void SetTypeIDForPickUp(int v) => typeIDForPickUp = v;

        GameObject body;
        public GameObject Body => body;

        public void Ctor() {
            body = transform.Find("body").gameObject;
        }

        public void SetMod(GameObject mod) {
            mod.transform.SetParent(body.transform, false);
        }

        public void SetPos(Vector3 pos) {
            transform.position = pos;
        }

        public void Hide() {
            gameObject.SetActive(false);
            TDLog.Log($"隐藏物品 类型: {ItemType}");
        }

        public void Show() {
            gameObject.SetActive(true);
            TDLog.Log($"显示物品 类型: {ItemType}");
        }

    }

}
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class ItemEntity : MonoBehaviour {

        int id;
        public int ID => id;
        public void SetID(int v) => id = v;

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

    }

}
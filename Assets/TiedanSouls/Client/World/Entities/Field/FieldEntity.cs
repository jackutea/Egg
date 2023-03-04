using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class FieldEntity : MonoBehaviour {

        int typeID;
        public int TypeID => typeID;
        public void SetTypeID(int v) => typeID = v;

        ushort chapter;
        public ushort Chapter => chapter;

        ushort level;
        public ushort Level => level;

        BoxCollider2D confiner;
        public Vector2 ConfinerSize => confiner.size;

        SpawnModel[] spawnModelArray;
        public SpawnModel[] SpawnModelArray => spawnModelArray;
        public void SetSpawnModelArray(SpawnModel[] v) => spawnModelArray = v;

        Vector2[] itemSpawnPosArray;
        public Vector2[] ItemSpawnPosArray => itemSpawnPosArray;
        public void SetItemSpawnPosArray(Vector2[] v) => itemSpawnPosArray = v;

        FieldType fieldType;
        public FieldType FieldType => fieldType;
        public void SetFieldType(FieldType v) => fieldType = v;

        // Field Door
        FieldDoorModel[] fieldDoorArray;
        public FieldDoorModel[] FieldDoorArray => fieldDoorArray;
        public void SetFieldDoorArray(FieldDoorModel[] v) => fieldDoorArray = v;

        // ====== Component
        FieldFSMComponent fsmComponent;
        public FieldFSMComponent FSMComponent => fsmComponent;

        public void Ctor() {
            confiner = transform.Find("confiner").GetComponent<BoxCollider2D>();
            TDLog.Assert(confiner != null);

            fsmComponent = new FieldFSMComponent();
        }

        public void Hide() {
            TDLog.Log($"隐藏场景: {name}");
            gameObject.SetActive(false);
        }

        public void Show() {
            TDLog.Log($"显示场景: {name}");
            gameObject.SetActive(true);
        }

        public bool TryFindDoorByIndex(int doorIndex, out FieldDoorModel door) {
            door = default;
            if (doorIndex < 0 || doorIndex >= fieldDoorArray.Length) {
                return false;
            }

            door = fieldDoorArray[doorIndex];
            return true;
        }

    }
}

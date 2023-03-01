using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class FieldEntity : MonoBehaviour {

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

        // ====== Component
        FieldFSMComponent fsmComponent;
        public FieldFSMComponent FSMComponent => fsmComponent;

        public void Ctor() {
            confiner = transform.Find("confiner").GetComponent<BoxCollider2D>();
            TDLog.Assert(confiner != null);

            fsmComponent = new FieldFSMComponent();
        }

        public void SetChapterAndLevel(ushort chapter, ushort level) {
            this.chapter = chapter;
            this.level = level;
        }

        public static uint GetID(ushort chapter, ushort level) {
            return (uint)(chapter << 16 | level);
        }

    }

}
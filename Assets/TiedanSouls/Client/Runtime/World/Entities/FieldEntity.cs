using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class FieldEntity : MonoBehaviour {

        ushort chapter;
        public ushort Chapter => chapter;

        ushort level;
        public ushort Level => level;

        BoxCollider2D confiner;
        public Vector2 ConfinerSize => confiner.size;

        public void Ctor() {
            confiner = transform.Find("confiner").GetComponent<BoxCollider2D>();
            TDLog.Assert(confiner != null);
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
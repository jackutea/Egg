using System.Collections.Generic;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World {

    public class FieldRepo {

        Dictionary<uint, FieldEntity> all;

        public FieldRepo() {
            all = new Dictionary<uint, FieldEntity>();
        }

        public bool TryGet(ushort chapter, ushort level, out FieldEntity field) {
            uint id = FieldEntity.GetID(chapter, level);
            return all.TryGetValue(id, out field);
        }

        public void Add(FieldEntity field) {
            uint id = FieldEntity.GetID(field.Chapter, field.Level);
            all.Add(id, field);
        }

        public void Remove(FieldEntity field) {
            uint id = FieldEntity.GetID(field.Chapter, field.Level);
            all.Remove(id);
        }

        public IEnumerable<FieldEntity> GetAll() {
            return all.Values;
        }

    }
}
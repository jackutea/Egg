using System.Collections.Generic;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World {

    public class FieldRepo {

        Dictionary<int, FieldEntity> all;

        public FieldRepo() {
            all = new Dictionary<int, FieldEntity>();
        }

        public bool TryGet(int typeID, out FieldEntity field) {
            return all.TryGetValue(typeID, out field);
        }

        public void Add(FieldEntity field) {
            all.Add(field.TypeID, field);
        }

        public void Remove(FieldEntity field) {
            all.Remove(field.TypeID);
        }

        public IEnumerable<FieldEntity> GetAll() {
            return all.Values;
        }

    }
}
using System.Collections.Generic;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public class FieldRepo {

        Dictionary<int, FieldEntity> all;

        public FieldRepo() {
            all = new Dictionary<int, FieldEntity>();
        }

        public bool TryGet(int typeID, out FieldEntity field) {
            return all.TryGetValue(typeID, out field);
        }

        public void Add(FieldEntity field) {
            all.Add(field.IDCom.TypeID, field);
        }

        public void Remove(FieldEntity field) {
            all.Remove(field.IDCom.TypeID);
        }

        public void Foreach(System.Action<FieldEntity> action) {
            var e = all.Values.GetEnumerator();
            while (e.MoveNext()) {
                action(e.Current);
            }
        }

    }
}
using System.Collections.Generic;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public class ProjectileRepo {

        Dictionary<int, ProjectileEntity> all;

        public ProjectileRepo() {
            all = new Dictionary<int, ProjectileEntity>();
        }

        public bool TryGet(int typeID, out ProjectileEntity field) {
            return all.TryGetValue(typeID, out field);
        }

        public void Add(ProjectileEntity field) {
            all.Add(field.IDCom.TypeID, field);
        }

        public void Remove(ProjectileEntity field) {
            all.Remove(field.IDCom.TypeID);
        }

    }
}
using System.Collections.Generic;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World {

    public class RoleRepository {

        Dictionary<int, RoleEntity> all;

        public RoleRepository() {
            all = new Dictionary<int, RoleEntity>();
        }

        public bool TryGet(int id, out RoleEntity role) {
            return all.TryGetValue(id, out role);
        }

        public void Add(RoleEntity role) {
            all.Add(role.ID, role);
        }

        public void Remove(RoleEntity role) {
            all.Remove(role.ID);
        }

        public IEnumerable<RoleEntity> GetAll() {
            return all.Values;
        }

    }
}
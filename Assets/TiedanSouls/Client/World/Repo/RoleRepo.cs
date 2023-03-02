using System;
using System.Collections.Generic;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World {

    public class RoleRepo {

        Dictionary<int, RoleEntity> all;
        public int Count => all.Count;

        RoleEntity playerRole;
        public RoleEntity PlayerRole => playerRole;
        public void SetPlayerRole(RoleEntity role) {
            playerRole = role;
        }

        public RoleRepo() {
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

        public void RemoveByID(int id) {
            all.Remove(id);
        }

        public void ForeachAll(Action<RoleEntity> action) {
            var e = all.Values.GetEnumerator();
            while (e.MoveNext()) {
                action.Invoke(e.Current);
            }
        }

    }
}
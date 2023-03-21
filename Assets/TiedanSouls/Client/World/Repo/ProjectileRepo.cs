using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public class ProjectileRepo {

        Dictionary<int, ProjectileEntity> all;

        public ProjectileRepo() {
            all = new Dictionary<int, ProjectileEntity>();
        }

        public bool TryGet(int id, out ProjectileEntity projectile) {
            return all.TryGetValue(id, out projectile);
        }

        public void Add(ProjectileEntity projectile) {
            all.Add(projectile.IDCom.EntityID, projectile);
        }

        public void Remove(ProjectileEntity projectile) {
            all.Remove(projectile.IDCom.EntityID);
        }

        public void Foreach(int curFieldTypeID, Action<ProjectileEntity> action) {
            var e = all.Values.GetEnumerator();
            if (curFieldTypeID == -1) {
                while (e.MoveNext()) {
                    var projectile = e.Current;
                    action(projectile);
                }
            } else {
                while (e.MoveNext()) {
                    var projectile = e.Current;
                    if (projectile.IDCom.FromFieldTypeID == curFieldTypeID) {
                        action(projectile);
                    }
                }
            }
        }

    }
}
using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    public class ProjectileRepo {

        Dictionary<int, ProjectileEntity> all;          // EntityID -> BulletEntity
        Dictionary<int, List<ProjectileEntity>> pool;   // EntityTypeID -> ProjectileEntity
        List<int> tempList;

        public ProjectileRepo() {
            all = new Dictionary<int, ProjectileEntity>();
            pool = new Dictionary<int, List<ProjectileEntity>>();
            tempList = new List<int>();
        }

        #region [增]

        public void Add(ProjectileEntity projectile) {
            all.Add(projectile.IDCom.EntityID, projectile);
        }

        public void AddToPool(ProjectileEntity projectile) {
            projectile.Reset();

            var idCom = projectile.IDCom;
            var key = idCom.TypeID;
            if (!pool.TryGetValue(key, out var list)) {
                list = new List<ProjectileEntity>();
                pool.Add(key, list);
            }
            list.Add(projectile);
            TDLog.Log($"弹幕仓库 添加到池 {idCom}");
        }

        #endregion

        #region [删]

        public bool TryRemove(ProjectileEntity projectile) {
            return all.Remove(projectile.IDCom.EntityID);
        }

        public bool TryRemove(int entityID) {
            return all.Remove(entityID);
        }

        #endregion

        #region [改]

        /// <summary>
        /// 移除某个关卡的弹幕到池中
        /// </summary>
        public void TearDownToPool(int fieldTypeID) {
            tempList.Clear();
            var e = all.Values.GetEnumerator();
            while (e.MoveNext()) {
                var projectile = e.Current;
                var idCom = projectile.IDCom;
                var fromFieldTypeID = idCom.FromFieldTypeID;
                if (fromFieldTypeID == fieldTypeID) {
                    projectile.TearDown();
                    this.AddToPool(projectile);
                    tempList.Add(projectile.IDCom.EntityID);
                }
            }

            tempList.ForEach(id => {
                this.TryRemove(id);
            });
        }

        #endregion

        #region [查]

        public bool TryGet(int id, out ProjectileEntity projectile) {
            return all.TryGetValue(id, out projectile);
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

        #endregion

    }

}
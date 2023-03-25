using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    public class BulletRepo {

        Dictionary<int, BulletEntity> all;

        List<int> removeList;

        public BulletRepo() {
            all = new Dictionary<int, BulletEntity>();
            removeList = new List<int>();
        }

        public bool TryGet(int id, out BulletEntity bullet) {
            return all.TryGetValue(id, out bullet);
        }

        public void Add(BulletEntity bullet) {
            var idCom = bullet.IDCom;
            all.Add(idCom.EntityID, bullet);
            TDLog.Log($"子弹仓库 添加 {idCom}");
        }

        public void Remove(BulletEntity bullet) {
            all.Remove(bullet.IDCom.EntityID);
        }

        public void Remove(int entityID) {
            all.Remove(entityID);
        }

        public void Foreach(int curFieldTypeID, Action<BulletEntity> action) {
            var e = all.Values.GetEnumerator();
            if (curFieldTypeID == -1) {
                while (e.MoveNext()) {
                    var bullet = e.Current;
                    action(bullet);
                }
            } else {
                while (e.MoveNext()) {
                    var bullet = e.Current;
                    if (bullet.IDCom.FromFieldTypeID == curFieldTypeID) {
                        action(bullet);
                    }
                }
            }
        }

        public List<int> ForeachAndGetRemoveList(int curFieldTypeID, Action<BulletEntity> action) {
            removeList.Clear();

            var e = all.Values.GetEnumerator();
            if (curFieldTypeID == -1) {
                while (e.MoveNext()) {
                    var bullet = e.Current;
                    action(bullet);
                    if (bullet.FSMCom.IsExiting) {
                        removeList.Add(bullet.IDCom.EntityID);
                    }
                }
                return removeList;
            }

            while (e.MoveNext()) {
                var bullet = e.Current;
                if (bullet.IDCom.FromFieldTypeID == curFieldTypeID) {
                    action(bullet);
                    if (bullet.FSMCom.IsExiting) {
                        removeList.Add(bullet.IDCom.EntityID);
                    }
                }
            }
            return removeList;
        }

    }
}
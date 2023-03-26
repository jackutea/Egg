using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    public class BulletRepo {

        Dictionary<int, BulletEntity> all;          // EntityID -> BulletEntity
        Dictionary<int, List<BulletEntity>> pool;   // EntityTypeID -> BulletEntity
        List<int> tempList;

        public BulletRepo() {
            all = new Dictionary<int, BulletEntity>();
            pool = new Dictionary<int, List<BulletEntity>>();
            tempList = new List<int>();
        }

        #region [增]

        public void Add(BulletEntity bullet) {
            var idCom = bullet.IDCom;
            var key = idCom.EntityID;
            all.Add(key, bullet);
            TDLog.Log($"子弹仓库 添加 {idCom}");
        }

        public void AddToPool(BulletEntity bullet) {
            var idCom = bullet.IDCom;
            var key = idCom.TypeID;
            if (!pool.TryGetValue(key, out var list)) {
                list = new List<BulletEntity>();
                pool.Add(key, list);
            }
            list.Add(bullet);
            TDLog.Log($"子弹仓库 添加到池 {idCom}");
        }

        #endregion

        #region [删]

        public bool TryRemove(BulletEntity bullet) {
            return all.Remove(bullet.IDCom.EntityID);
        }

        public bool TryRemove(int entityID) {
            return all.Remove(entityID);
        }

        #endregion

        #region [改]

        /// <summary>
        /// 移除某个关卡的子弹到池中
        /// </summary>
        public void TearDownToPool(int fieldTypeID) {
            tempList.Clear();
            var e = all.Values.GetEnumerator();
            while (e.MoveNext()) {
                var bullet = e.Current;
                var idCom = bullet.IDCom;
                var fromFieldTypeID = idCom.FromFieldTypeID;
                if (fromFieldTypeID == fieldTypeID) {
                    bullet.TearDown();
                    this.AddToPool(bullet);
                    tempList.Add(bullet.IDCom.EntityID);
                }
            }

            tempList.ForEach(id => {
                this.TryRemove(id);
            });
        }

        public void TearDownToPool_NoneState() {
            tempList.Clear();
            var e = all.Values.GetEnumerator();
            while (e.MoveNext()) {
                var bullet = e.Current;
                var fsm = bullet.FSMCom;
                if (fsm.State == BulletFSMState.None) {
                    bullet.TearDown();
                    this.AddToPool(bullet);
                    tempList.Add(bullet.IDCom.EntityID);
                }
            }

            tempList.ForEach(id => {
                this.TryRemove(id);
            });
        }

        #endregion

        #region [查]

        public bool TryGet(int entityID, out BulletEntity bullet) {
            return all.TryGetValue(entityID, out bullet);
        }

        public bool TryGetFromPool(int typeID, out BulletEntity bullet) {
            if (pool.TryGetValue(typeID, out var list)) {
                var index = list.Count - 1;
                if (index >= 0) {
                    bullet = list[index];
                    list.RemoveAt(index);
                    TDLog.Log($"子弹仓库 从池中取出子弹 类型ID {typeID}");
                    return true;
                }
            }
            bullet = null;
            return false;
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

        #endregion

    }
}
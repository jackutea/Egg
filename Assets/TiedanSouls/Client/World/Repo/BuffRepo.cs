using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    public class BuffRepo {

        Dictionary<int, BuffEntity> all;          // EntityID -> BuffEntity
        Dictionary<int, List<BuffEntity>> pool;   // EntityTypeID -> BuffEntity
        List<int> tempList;

        public BuffRepo() {
            all = new Dictionary<int, BuffEntity>();
            pool = new Dictionary<int, List<BuffEntity>>();
            tempList = new List<int>();
        }

        #region [增]

        public void Add(BuffEntity buff) {
            var idCom = buff.IDCom;
            var key = idCom.EntityID;
            all.Add(key, buff);
            TDLog.Log($"buff仓库 添加 {idCom.EntityName}");
        }

        public void AddToPool(BuffEntity buff) {
            var idCom = buff.IDCom;
            var key = idCom.TypeID;
            if (!pool.TryGetValue(key, out var list)) {
                list = new List<BuffEntity>();
                pool.Add(key, list);
            }
            list.Add(buff);
            TDLog.Log($"buff仓库 添加到池 {idCom.EntityName}");
        }

        #endregion

        #region [删]

        public bool TryRemove(BuffEntity buff) {
            return all.Remove(buff.IDCom.EntityID);
        }

        public bool TryRemove(int entityID, out BuffEntity buff) {
            if (all.TryGetValue(entityID, out buff)) {
                all.Remove(entityID);
                return true;
            }
            return false;
        }

        #endregion

        #region [查]

        public bool TryGet(int entityID, out BuffEntity buff) {
            return all.TryGetValue(entityID, out buff);
        }

        public bool TryGetFromPool(int typeID, out BuffEntity buff) {
            if (pool.TryGetValue(typeID, out var list)) {
                var index = list.Count - 1;
                if (index >= 0) {
                    buff = list[index];
                    list.RemoveAt(index);
                    TDLog.Log($"buff仓库 从池中取出buff 类型ID {typeID}");
                    return true;
                }
            }
            buff = null;
            return false;
        }

        #endregion

    }
}
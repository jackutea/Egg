using System;
using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class BuffSlotComponent {

        Dictionary<int, List<BuffEntity>> buffDic;

        List<int> removeList;

        public BuffSlotComponent() {
            buffDic = new Dictionary<int, List<BuffEntity>>();
            removeList = new List<int>();
        }

        #region [增加]

        public void Add(BuffEntity BuffEntity) {
            var idCom = BuffEntity.IDCom;
            var typeID = idCom.TypeID;
            if (!buffDic.TryGetValue(typeID, out var list)) {
                list = new List<BuffEntity>();
                buffDic.Add(typeID, list);
            }

            list.Add(BuffEntity);
            TDLog.Log($"BuffSlotComponent 添加 Buff: {BuffEntity.IDCom.TypeID}");
        }

        #endregion

        #region [删除]

        public void Remove(BuffEntity BuffEntity) {
            var idCom = BuffEntity.IDCom;
            var typeID = idCom.TypeID;
            if (!buffDic.TryGetValue(typeID, out var list)) {
                return;
            }

            list.Remove(BuffEntity);
            TDLog.Log($"BuffSlotComponent 移除 Buff: {BuffEntity.IDCom.TypeID}");
        }

        #endregion

        #region [改]

        public void SetFather(EntityIDArgs father) {
            var e = buffDic.Values.GetEnumerator();
            while (e.MoveNext()) {
                var buffList = e.Current;
                buffList.ForEach((buff) => {
                    buff.IDCom.SetFather(father);
                });
            }
        }


        #endregion

        #region [查询]

        public void Foreach(Action<BuffEntity> action) {
            var e = buffDic.Values.GetEnumerator();
            while (e.MoveNext()) {
                var buffList = e.Current;
                buffList.ForEach((buff) => {
                    action(buff);
                });
            }
        }

        public List<int> ForeachAndGetRemoveList(Action<BuffEntity> action) {
            removeList.Clear();
            var e = buffDic.Values.GetEnumerator();
            while (e.MoveNext()) {
                var buffList = e.Current;
                buffList.ForEach((buff) => {
                    if (buff.IsFinished()) {
                        removeList.Add(buff.IDCom.TypeID);
                    } else {
                        action(buff);
                    }
                });
            }
            return removeList;
        }

        public bool HasSameTypeBuff(int typeID) {
            return buffDic.ContainsKey(typeID);
        }

        #endregion

    }

}
using System;
using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class BuffSlotComponent {

        Dictionary<int, List<BuffEntity>> buffDic;

        List<BuffEntity> removeList;

        public BuffSlotComponent() {
            buffDic = new Dictionary<int, List<BuffEntity>>();
            removeList = new List<BuffEntity>();
        }

        #region [增加]

        public void Add(BuffEntity buff) {
            var idCom = buff.IDCom;
            var typeID = idCom.TypeID;
            if (!buffDic.TryGetValue(typeID, out var list)) {
                list = new List<BuffEntity>();
                buffDic.Add(typeID, list);
            }

            list.Add(buff);
            TDLog.Log($"BuffSlotComponent 添加 Buff\n{buff.IDCom}\n{buff.Description}");
        }

        #endregion

        #region [删除]

        public void Remove(BuffEntity buff) {
            var idCom = buff.IDCom;
            var typeID = idCom.TypeID;
            if (!buffDic.TryGetValue(typeID, out var list)) {
                return;
            }

            list.Remove(buff);
            TDLog.Log($"BuffSlotComponent 移除 Buff\n{buff.IDCom}\n{buff.Description}");
        }

        public void RemoveByEntityID(int entityID) {
            var e = buffDic.Values.GetEnumerator();
            while (e.MoveNext()) {
                var buffList = e.Current;
                var count = buffList.Count;
                for (int i = 0; i < count; i++) {
                    var buff = buffList[i];
                    if (buff.IDCom.EntityID == entityID) {
                        buffList.RemoveAt(i);
                        TDLog.Log($"BuffSlotComponent 移除 Buff\n{buff.IDCom.TypeID}\n{buff.Description}");
                        break;
                    }
                }
            }
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

        public List<BuffEntity> ForeachAndGetRemoveList(Action<BuffEntity> action) {
            removeList.Clear();
            var e = buffDic.Values.GetEnumerator();
            while (e.MoveNext()) {
                var buffList = e.Current;
                buffList.ForEach((buff) => {
                    if (buff.IsFinished()) {
                        removeList.Add(buff);
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
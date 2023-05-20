using System;
using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class BuffSlotComponent {

        Dictionary<int, BuffEntity> buffDic;

        List<BuffEntity> removeList;

        public BuffSlotComponent() {
            buffDic = new Dictionary<int, BuffEntity>();
            removeList = new List<BuffEntity>();
        }

        public bool TryAdd(BuffEntity buff) {
            var idCom = buff.IDCom;
            var typeID = idCom.TypeID;
            if (!buffDic.TryAdd((int)typeID, buff)) {
                return false;
            }

            TDLog.Log($"BuffSlotComponent 添加 Buff\n{buff.IDCom}\n{buff.Description}");
            return true;
        }

        public bool TryRemove(BuffEntity buff) {
            var idCom = buff.IDCom;
            var typeID = idCom.TypeID;
            if (!buffDic.Remove(typeID)) {
                return false;
            }

            TDLog.Log($"BuffSlotComponent 移除 Buff\n{buff.IDCom}\n{buff.Description}");
            return true;
        }

        public bool TryGet(int typeID, out BuffEntity buff) {
            return buffDic.TryGetValue(typeID, out buff);
        }

        public void SetFather(EntityIDArgs father) {
            var e = buffDic.Values.GetEnumerator();
            while (e.MoveNext()) {
                var buff = e.Current;
                buff.IDCom.SetFather(father);
            }
        }

        public void Foreach(Action<BuffEntity> action) {
            var e = buffDic.Values.GetEnumerator();
            while (e.MoveNext()) {
                var buff = e.Current;
                action(buff);
            }
        }

        public List<BuffEntity> ForeachAndGetRemoveList(Action<BuffEntity> action) {
            removeList.Clear();
            var e = buffDic.Values.GetEnumerator();
            while (e.MoveNext()) {
                var buff = e.Current;
                if (buff.IsFinished()) {
                    removeList.Add(buff);
                    continue;
                }

                action(buff);
            }
            return removeList;
        }

    }

}
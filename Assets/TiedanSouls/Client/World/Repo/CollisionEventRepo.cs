using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    public class CollisionEventRepo {

        List<CollisionEventModel> events_triggerEnter;
        List<CollisionEventModel> events_triggerExit;

        public CollisionEventRepo() {
            events_triggerEnter = new List<CollisionEventModel>();
            events_triggerExit = new List<CollisionEventModel>();
        }

        public void Add_TriggerEnter(in CollisionEventModel args) {
            events_triggerEnter.Add(args);
            TDLog.Log($"碰撞事件仓库 添加 <Trigger:Enter>\n{args} ");
        }

        public void Add_TriggerExit(in CollisionEventModel args) {
            events_triggerExit.Add(args);
            TDLog.Log($"碰撞事件仓库 添加 <Trigger:Exit>\n{args} ");
        }

        public bool TryPick_Enter(out CollisionEventModel args) {
            args = default;

            var count = events_triggerEnter.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = events_triggerEnter[index];
            events_triggerEnter.RemoveAt(index);
            return true;
        }

        public bool TryPick_Exit(out CollisionEventModel args) {
            args = default;

            var count = events_triggerExit.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = events_triggerExit[index];
            events_triggerExit.RemoveAt(index);
            return true;
        }

    }

}
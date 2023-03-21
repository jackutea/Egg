using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    public class CollisionEventRepo {

        List<CollisionEventArgs> events_triggerEnter;
        List<CollisionEventArgs> events_triggerExit;

        public CollisionEventRepo() {
            events_triggerEnter = new List<CollisionEventArgs>();
            events_triggerExit = new List<CollisionEventArgs>();
        }

        public void Add_TriggerEnter(in CollisionEventArgs args) {
            events_triggerEnter.Add(args);
            TDLog.Log($"碰撞事件<Trigger:Enter>\n{args} ");
        }

        public void Add_TriggerExit(in CollisionEventArgs args) {
            events_triggerExit.Add(args);
            TDLog.Log($"碰撞事件<Trigger:Exit>\n{args} ");
        }

        public bool TryPick_Enter(out CollisionEventArgs args) {
            args = default;

            var count = events_triggerEnter.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = events_triggerEnter[index];
            events_triggerEnter.RemoveAt(index);
            TDLog.Log($"TryPick_Enter\n{args} ");
            return true;
        }

        public bool TryPick_Exit(out CollisionEventArgs args) {
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
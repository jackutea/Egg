using System.Collections.Generic;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public class CollisionEventRepo {

        List<CollisionEventArgs> events_triggerEnter;
        List<CollisionEventArgs> events_triggerExit;
        List<CollisionEventArgs> events_triggerStay;

        public CollisionEventRepo() {
            events_triggerEnter = new List<CollisionEventArgs>();
            events_triggerExit = new List<CollisionEventArgs>();
            events_triggerStay = new List<CollisionEventArgs>();
        }

        public void Add_TriggerEnter(in CollisionEventArgs args) {
            events_triggerEnter.Add(args);
            TDLog.Log($"碰撞事件 TriggerEnter  ");
        }

        public void Add_TriggerExit(in CollisionEventArgs args) {
            events_triggerExit.Add(args);
        }

        public void Add_TriggerStay(in CollisionEventArgs args) {
            events_triggerStay.Add(args);
        }

        public bool TryPick_EnterEvent(out CollisionEventArgs args) {
            args = default;

            var count = events_triggerEnter.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = events_triggerEnter[index];
            events_triggerEnter.RemoveAt(index);
            return true;
        }

        public bool TryPick_ExitEvent(out CollisionEventArgs args) {
            args = default;

            var count = events_triggerExit.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = events_triggerExit[index];
            events_triggerExit.RemoveAt(index);
            return true;
        }

        public bool TryPick_StayEvent(out CollisionEventArgs args) {
            args = default;

            var count = events_triggerStay.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = events_triggerStay[index];
            events_triggerStay.RemoveAt(index);
            return true;
        }

    }

}
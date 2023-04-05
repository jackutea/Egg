using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    public class CollisionEventRepo {

        Dictionary<long, CollisionEventModel> triggerEvents;
        Dictionary<long, CollisionEventModel> collisionEvents;

        List<CollisionEventModel> triggerEnterEvents;
        List<CollisionEventModel> triggerStayEvents;
        List<CollisionEventModel> triggerExitEvents;

        List<CollisionEventModel> collisionEnterEvents;
        List<CollisionEventModel> collisionStayEvents;
        List<CollisionEventModel> collisionExitEvents;

        public CollisionEventRepo() {
            triggerEnterEvents = new List<CollisionEventModel>();
            triggerExitEvents = new List<CollisionEventModel>();
            triggerStayEvents = new List<CollisionEventModel>();
            collisionEnterEvents = new List<CollisionEventModel>();
            collisionExitEvents = new List<CollisionEventModel>();
            collisionStayEvents = new List<CollisionEventModel>();
        }

        public void Add_TriggerEnter(in CollisionEventModel args) {
            triggerEnterEvents.Add(args);
            TDLog.Log($"碰撞事件仓库 添加 <Trigger:Enter>\n{args} ");
        }

        public void Add_TriggerStay(in CollisionEventModel args) {
            triggerStayEvents.Add(args);
            TDLog.Log($"碰撞事件仓库 添加 <Trigger:Stay>\n{args} ");
        }

        public void Add_TriggerExit(in CollisionEventModel args) {
            triggerExitEvents.Add(args);
            TDLog.Log($"碰撞事件仓库 添加 <Trigger:Exit>\n{args} ");
        }

        public void Add_CollisionEnter(in CollisionEventModel args) {
            collisionEnterEvents.Add(args);
            TDLog.Log($"碰撞事件仓库 添加 <Collision:Enter>\n{args} ");
        }

        public void Add_CollisionStay(in CollisionEventModel args) {
            collisionStayEvents.Add(args);
            TDLog.Log($"碰撞事件仓库 添加 <Collision:Stay>\n{args} ");
        }

        public void Add_CollisionExit(in CollisionEventModel args) {
            collisionExitEvents.Add(args);
            TDLog.Log($"碰撞事件仓库 添加 <Collision:Exit>\n{args} ");
        }

        public bool TryPick_TriggerEnter(out CollisionEventModel args) {
            args = default;

            var count = triggerEnterEvents.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = triggerEnterEvents[index];
            triggerEnterEvents.RemoveAt(index);
            return true;
        }

        public bool TryPick_TriggerStay(out CollisionEventModel args) {
            args = default;

            var count = triggerStayEvents.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = triggerStayEvents[index];
            triggerStayEvents.RemoveAt(index);
            return true;
        }

        public bool TryPick_TriggerExit(out CollisionEventModel args) {
            args = default;

            var count = triggerExitEvents.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = triggerExitEvents[index];
            triggerExitEvents.RemoveAt(index);
            return true;
        }

        public bool TryPick_CollisionEnter(out CollisionEventModel args) {
            args = default;

            var count = collisionEnterEvents.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = collisionEnterEvents[index];
            collisionEnterEvents.RemoveAt(index);
            return true;
        }

        public bool TryPick_CollisionStay(out CollisionEventModel args) {
            args = default;

            var count = collisionStayEvents.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = collisionStayEvents[index];
            collisionStayEvents.RemoveAt(index);
            return true;
        }

        public bool TryPick_CollisionExit(out CollisionEventModel args) {
            args = default;

            var count = collisionExitEvents.Count;
            var index = count - 1;
            if (index < 0) return false;

            args = collisionExitEvents[index];
            collisionExitEvents.RemoveAt(index);
            return true;
        }

        public long GetKey(in EntityIDArgs args1, in EntityIDArgs args2) {
            short entityType1 = (short)args1.entityType;
            short entityID1 = args1.entityID;
            int key1 = (int)entityType1 << 16;
            key1 |= (int)entityID1;

            short entityType2 = (short)args2.entityType;
            short entityID2 = args2.entityID;
            int key2 = (int)entityType2 << 16;
            key2 |= (int)entityID2;

            Swap(ref key1, ref key2);

            return (long)key1 << 32;
        }

        void Swap(ref int key1, ref int key2) {
            if (key1 < key2) {
                key1 = key1 ^ key2;
                key2 = key1 ^ key2;
                key1 = key1 ^ key2;
            }
        }

    }

}
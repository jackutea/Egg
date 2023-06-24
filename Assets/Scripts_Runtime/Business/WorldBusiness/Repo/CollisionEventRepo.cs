using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client {

    public class CollisionEventRepo {

        Dictionary<ulong, EntityCollisionEvent> triggerEnterEvents;
        Dictionary<ulong, EntityCollisionEvent> triggerStayEvents;
        Dictionary<ulong, EntityCollisionEvent> triggerExitEvents;
        Dictionary<ulong, EntityCollisionEvent> collisionEnterEvents;
        Dictionary<ulong, EntityCollisionEvent> collisionStayEvents;
        Dictionary<ulong, EntityCollisionEvent> collisionExitEvents;

        public CollisionEventRepo() {
            triggerEnterEvents = new Dictionary<ulong, EntityCollisionEvent>();
            triggerStayEvents = new Dictionary<ulong, EntityCollisionEvent>();
            triggerExitEvents = new Dictionary<ulong, EntityCollisionEvent>();
            collisionEnterEvents = new Dictionary<ulong, EntityCollisionEvent>();
            collisionStayEvents = new Dictionary<ulong, EntityCollisionEvent>();
            collisionExitEvents = new Dictionary<ulong, EntityCollisionEvent>();
        }

        public void Add_TriggerEnter(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB) {
            var key = GetKey(entityColliderModelA.HolderIDCom, entityColliderModelB.HolderIDCom);
            EntityCollisionEvent eventModel;
            eventModel.entityColliderModelA = entityColliderModelA;
            eventModel.entityColliderModelB = entityColliderModelB;
            eventModel.normalA = Vector3.zero;
            eventModel.normalB = Vector3.zero;
            triggerEnterEvents.Add(key, eventModel);
        }

        public void Add_TriggerStay(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB) {
            var key = GetKey(entityColliderModelA.HolderIDCom, entityColliderModelB.HolderIDCom);
            EntityCollisionEvent eventModel;
            eventModel.entityColliderModelA = entityColliderModelA;
            eventModel.entityColliderModelB = entityColliderModelB;
            eventModel.normalA = Vector3.zero;
            eventModel.normalB = Vector3.zero;
            triggerStayEvents.Add(key, eventModel);
        }

        public void Add_TriggerExit(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB) {
            var key = GetKey(entityColliderModelA.HolderIDCom, entityColliderModelB.HolderIDCom);
            EntityCollisionEvent eventModel;
            eventModel.entityColliderModelA = entityColliderModelA;
            eventModel.entityColliderModelB = entityColliderModelB;
            eventModel.normalA = Vector3.zero;
            eventModel.normalB = Vector3.zero;
            triggerExitEvents.Add(key, eventModel);
        }

        public void Add_CollisionEnter(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB, Vector3 normalB) {
            var key = GetKey(entityColliderModelA.HolderIDCom, entityColliderModelB.HolderIDCom);
            EntityCollisionEvent eventModel = new EntityCollisionEvent();
            eventModel.entityColliderModelA = entityColliderModelA;
            eventModel.entityColliderModelB = entityColliderModelB;
            eventModel.normalA = -normalB;
            eventModel.normalB = normalB;
            collisionEnterEvents.Add(key, eventModel);
        }

        public void Add_CollisionStay(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB, Vector3 normalB) {
            var key = GetKey(entityColliderModelA.HolderIDCom, entityColliderModelB.HolderIDCom);
            EntityCollisionEvent eventModel = new EntityCollisionEvent();
            eventModel.entityColliderModelA = entityColliderModelA;
            eventModel.entityColliderModelB = entityColliderModelB;
            eventModel.normalA = -normalB;
            eventModel.normalB = normalB;
            collisionStayEvents.Add(key, eventModel);
        }

        public void Add_CollisionExit(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB) {
            var key = GetKey(entityColliderModelA.HolderIDCom, entityColliderModelB.HolderIDCom);
            EntityCollisionEvent eventModel = new EntityCollisionEvent();
            eventModel.entityColliderModelA = entityColliderModelA;
            eventModel.entityColliderModelB = entityColliderModelB;
            eventModel.normalA = Vector3.zero;
            eventModel.normalB = Vector3.zero;
            collisionExitEvents.Add(key, eventModel);
        }

        public void Foreach_TriggerEnter(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in triggerEnterEvents.Values) {
                action(eventModel);
            }
        }

        public void Foreach_TriggerStay(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in triggerStayEvents.Values) {
                action(eventModel);
            }
        }

        public void Foreach_TriggerExit(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in triggerExitEvents.Values) {
                action(eventModel);
            }
        }

        public void Foreach_CollisionEnter(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in collisionEnterEvents.Values) {
                action(eventModel);
            }
        }

        public void Foreach_CollisionStay(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in collisionEnterEvents.Values) {
                action(eventModel);
            }
        }

        public void Foreach_CollisionExit(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in collisionExitEvents.Values) {
                action(eventModel);
            }
        }

        public ulong GetKey(in EntityIDComponent a, in EntityIDComponent b) {
            return GetKey(a.EntityType, a.EntityID, b.EntityType, b.EntityID);
        }

        public ulong GetKey(EntityType typeA, short idA, EntityType typeB, short idB) {

            uint key1 = (uint)(ushort)typeA << 16;
            key1 |= (uint)(ushort)idA;

            uint key2 = (uint)(ushort)typeB << 16;
            key2 |= (uint)(ushort)idB;

            SwapMajorToLeft(ref key1, ref key2);

            ulong key = (ulong)key1 << 32;
            key |= (ulong)key2;

            return key;
        }

        void SwapMajorToLeft(ref uint key1, ref uint key2) {
            if (key1 < key2) {
                // eg: key1 = 0110, key2 = 1010
                // key1 = key1 ^ key2 = 1100
                // key2 = key1 ^ key2 = 0110
                // key1 = key1 ^ key2 = 1010
                key1 = key1 ^ key2;
                key2 = key1 ^ key2;
                key1 = key1 ^ key2;
            }
        }

    }

}
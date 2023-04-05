using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client {

    public class CollisionEventRepo {

        Dictionary<ulong, EntityCollisionEvent> triggerEvents;
        Dictionary<ulong, EntityCollisionEvent> collisionEvents;

        public CollisionEventRepo() {
            triggerEvents = new Dictionary<ulong, EntityCollisionEvent>();
            collisionEvents = new Dictionary<ulong, EntityCollisionEvent>();
        }

        public void Update() {
            foreach (var ev in triggerEvents.Values) {
                ev.triggerState = ev.triggerState == TriggerState.Exit ? TriggerState.None : ev.triggerState;
                ev.lastTriggerState = ev.triggerState;
            }
            foreach (var ev in collisionEvents.Values) {
                ev.triggerState = ev.triggerState == TriggerState.Exit ? TriggerState.None : ev.triggerState;
                ev.lastTriggerState = ev.triggerState;
            }
        }

        public void Add_TriggerEnter(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB, Vector3 normalA) {
            var key = GetKey(entityColliderModelA.Father, entityColliderModelB.Father);
            if (!triggerEvents.TryGetValue(key, out var eventModel)) {
                eventModel = new EntityCollisionEvent();
                eventModel.entityColliderModelA = entityColliderModelA;
                eventModel.entityColliderModelB = entityColliderModelB;
                eventModel.normalA = normalA;
                eventModel.normalB = -normalA;
                eventModel.triggerState = TriggerState.Enter;
                triggerEvents.Add(key, eventModel);
            } else {
                eventModel.triggerState = eventModel.lastTriggerState == TriggerState.None ? TriggerState.Enter : TriggerState.Stay;
            }
        }

        public void Add_TriggerExit(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB) {
            var key = GetKey(entityColliderModelA.Father, entityColliderModelB.Father);
            if (triggerEvents.TryGetValue(key, out var eventModel)) {
                eventModel.triggerState = TriggerState.Exit;
            }
        }

        public void Add_TriggerStay(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB, Vector3 normalA) {
            var key = GetKey(entityColliderModelA.Father, entityColliderModelB.Father);
            if (triggerEvents.TryGetValue(key, out var eventModel)) {
                eventModel.triggerState = TriggerState.Stay;
            }
        }

        public void Add_CollisionEnter(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB, Vector3 normalA) {
            var key = GetKey(entityColliderModelA.Father, entityColliderModelB.Father);
            if (!collisionEvents.TryGetValue(key, out var eventModel)) {
                eventModel = new EntityCollisionEvent();
                eventModel.entityColliderModelA = entityColliderModelA;
                eventModel.entityColliderModelB = entityColliderModelB;
                eventModel.normalA = normalA;
                eventModel.normalB = -normalA;
                eventModel.triggerState = TriggerState.Enter;
                collisionEvents.Add(key, eventModel);
            } else {
                eventModel.triggerState = eventModel.lastTriggerState == TriggerState.None ? TriggerState.Enter : TriggerState.Stay;
            }
        }

        public void Add_CollisionExit(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB) {
            var key = GetKey(entityColliderModelA.Father, entityColliderModelB.Father);
            if (collisionEvents.TryGetValue(key, out var eventModel)) {
                eventModel.triggerState = TriggerState.Exit;
            }
        }

        public void Add_CollisionStay(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB, Vector3 normalA) {
            var key = GetKey(entityColliderModelA.Father, entityColliderModelB.Father);
            if (collisionEvents.TryGetValue(key, out var eventModel)) {
                eventModel.triggerState = TriggerState.Stay;
            }
        }

        public void Foreach_TriggerEnter(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in triggerEvents.Values) {
                if (eventModel.triggerState == TriggerState.Enter) {
                    action(eventModel);
                }
            }
        }

        public void Foreach_TriggerStay(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in triggerEvents.Values) {
                if (eventModel.triggerState == TriggerState.Stay) {
                    action(eventModel);
                }
            }
        }

        public void Foreach_TriggerExit(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in triggerEvents.Values) {
                if (eventModel.triggerState == TriggerState.Exit) {
                    action(eventModel);
                    eventModel.triggerState = TriggerState.None;
                }
            }
        }

        public void Foreach_CollisionEnter(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in collisionEvents.Values) {
                if (eventModel.triggerState == TriggerState.Enter) {
                    action(eventModel);
                }
            }
        }

        public void Foreach_CollisionStay(InAction<EntityCollisionEvent> action) {
            foreach (var eventModel in collisionEvents.Values) {
                if (eventModel.triggerState == TriggerState.Stay) {
                    action(eventModel);
                }
            }
        }

        public void Foreach_CollisionExit(InAction<EntityCollisionEvent> action) {

            foreach (var eventModel in collisionEvents.Values) {
                if (eventModel.triggerState == TriggerState.Exit) {
                    action(eventModel);
                }
            }
        }

        public ulong GetKey(in EntityIDArgs args1, in EntityIDArgs args2) {
            ushort entityType1 = (ushort)args1.entityType;
            ushort entityID1 = (ushort)args1.entityID;
            uint key1 = (uint)entityType1 << 16;
            key1 |= (uint)entityID1;

            ushort entityType2 = (ushort)args2.entityType;
            ushort entityID2 = (ushort)args2.entityID;
            uint key2 = (uint)entityType2 << 16;
            key2 |= (uint)entityID2;

            Swap(ref key1, ref key2);

            ulong key = (ulong)key1 << 32;
            key |= (ulong)key2;

            return key;
        }

        void Swap(ref uint key1, ref uint key2) {
            if (key1 < key2) {
                key1 = key1 ^ key2;
                key2 = key1 ^ key2;
                key1 = key1 ^ key2;
            }
        }

    }

}
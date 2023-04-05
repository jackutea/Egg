using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 实体碰撞器
    /// </summary>
    public class EntityCollider : MonoBehaviour {

        EntityIDArgs father;
        public EntityIDArgs Father => father;

        TargetGroupType hitTargetGroupType;
        public TargetGroupType HitTargetGroupType => hitTargetGroupType;
        public void SetHitTargetGroupType(TargetGroupType v) => hitTargetGroupType = v;

        ColliderModel colliderModel;
        public ColliderModel ColliderModel => colliderModel;
        public void SetColliderModel(ColliderModel v) => colliderModel = v;

        // 碰撞检测事件
        public delegate void CollisionEventHandler(in CollisionEventModel args);
        public CollisionEventHandler onTriggerEnter2D;
        public CollisionEventHandler onTriggerStay2D;
        public CollisionEventHandler onTriggerExit2D;
        public CollisionEventHandler onCollisionEnter2D;
        public CollisionEventHandler onCollisionStay2D;
        public CollisionEventHandler onCollisionExit2D;

        public bool IsActivated => gameObject.activeSelf;

        public void Activate() {
            gameObject.SetActive(true);
        }

        public void Deactivate() {
            gameObject.SetActive(false);
        }

        public void SetFather(EntityIDArgs father) {
            this.father = father;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            CollisionEventModel args;
            args.entityColliderModelA = this;
            args.entityColliderModelB = otherColliderModel;
            args.normalA = Vector3.zero;
            onTriggerEnter2D?.Invoke(args);
        }

        void OnTriggerStay2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            CollisionEventModel args;
            args.entityColliderModelA = this;
            args.entityColliderModelB = otherColliderModel;
            args.normalA = Vector3.zero;
            onTriggerStay2D?.Invoke(args);
        }

        void OnTriggerExit2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            CollisionEventModel args;
            args.entityColliderModelA = this;
            args.entityColliderModelB = otherColliderModel;
            args.normalA = Vector3.zero;
            onTriggerExit2D?.Invoke(args);
        }

        void OnCollisionEnter2D(Collision2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            CollisionEventModel args;
            args.entityColliderModelA = this;
            args.entityColliderModelB = otherColliderModel;
            args.normalA = other.GetContact(0).normal;
            onCollisionEnter2D?.Invoke(args);
        }

        void OnCollisionStay2D(Collision2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            CollisionEventModel args;
            args.entityColliderModelA = this;
            args.entityColliderModelB = otherColliderModel;
            args.normalA = other.GetContact(0).normal;
            onCollisionStay2D?.Invoke(args);
        }

        void OnCollisionExit2D(Collision2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            CollisionEventModel args;
            args.entityColliderModelA = this;
            args.entityColliderModelB = otherColliderModel;
            args.normalA = Vector3.zero;
            onCollisionExit2D?.Invoke(args);
        }

    }

}
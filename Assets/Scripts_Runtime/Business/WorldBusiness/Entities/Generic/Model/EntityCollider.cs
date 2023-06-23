using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 实体碰撞器
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class EntityCollider : MonoBehaviour {

        EntityIDComponent holderIDCom;
        public EntityIDComponent HolderIDCom => holderIDCom;
        public void SetHolder(EntityIDComponent v) => holderIDCom = v;

        Collider2D coll;
        public Collider2D Coll => coll;

        ColliderModel colliderModel;
        public ColliderModel ColliderModel => colliderModel;
        public void SetColliderModel(ColliderModel v) => colliderModel = v;

        // 碰撞检测事件
        public delegate void CollisionEventHandler(EntityCollider entityColliderModelA, EntityCollider entityColliderModelB, Vector3 normalA);
        public CollisionEventHandler onTriggerEnter2D;
        public CollisionEventHandler onTriggerStay2D;
        public CollisionEventHandler onTriggerExit2D;
        public CollisionEventHandler onCollisionEnter2D;
        public CollisionEventHandler onCollisionStay2D;
        public CollisionEventHandler onCollisionExit2D;

        public void Ctor() {
            coll = GetComponent<Collider2D>();
        }

        public void SetActive(bool isActive) {
            gameObject.SetActive(isActive);
        }

        public void Activate() {
            gameObject.SetActive(true);
        }

        public void Deactivate() {
            gameObject.SetActive(false);
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            onTriggerEnter2D?.Invoke(this, otherColliderModel, Vector3.zero);
        }

        void OnTriggerStay2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            onTriggerStay2D?.Invoke(this, otherColliderModel, Vector3.zero);
        }

        void OnTriggerExit2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            onTriggerExit2D?.Invoke(this, otherColliderModel, Vector3.zero);
        }

        void OnCollisionEnter2D(Collision2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            onCollisionEnter2D?.Invoke(this, otherColliderModel, other.GetContact(0).normal);
        }

        void OnCollisionStay2D(Collision2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            onCollisionStay2D?.Invoke(this, otherColliderModel, other.GetContact(0).normal);
        }

        void OnCollisionExit2D(Collision2D other) {
            if (!other.gameObject.TryGetComponent<EntityCollider>(out var otherColliderModel)) return;
            onCollisionExit2D?.Invoke(this, otherColliderModel, Vector3.zero);
        }

    }

}
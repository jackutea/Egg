using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 实体碰撞器
    /// </summary>
    public class EntityCollider : MonoBehaviour {

        EntityIDArgs father;
        public EntityIDArgs Father => father;

        AllyType hitAllyType;
        public AllyType HitTargetGroupType => hitAllyType;
        public void SetHitTargetGroupType(AllyType v) => hitAllyType = v;

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
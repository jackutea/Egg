using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 实体碰撞器模型
    /// </summary>
    public class EntityColliderModel : MonoBehaviour {

        [SerializeField] EntityIDArgs father;
        public EntityIDArgs Father => father;

        ColliderType colliderType;
        public ColliderType ColliderType => colliderType;
        public void SetColliderType(ColliderType colliderType) => this.colliderType = colliderType;

        Vector3 localPos;
        public Vector3 LocalPos => localPos;
        public void SetLocalPos(Vector3 localPos) => this.localPos = localPos;

        float localAngleZ;
        public float LocalAngleZ => localAngleZ;
        public void SetLocalAngleZ(float localAngleZ) => this.localAngleZ = localAngleZ;

        Vector3 localScale;
        public Vector3 LocalScale => localScale;
        public void SetLocalScale(Vector3 localScale) => this.localScale = localScale;

        Vector3 size;
        public Vector3 Size => size;
        public void SetSize(Vector3 size) => this.size = size;

        // 打击目标类型
        RelativeTargetGroupType hitRelativeTargetGroupType;
        public void SetHitRelativeTargetGroupType(RelativeTargetGroupType hitRelativeTargetGroupType) => this.hitRelativeTargetGroupType = hitRelativeTargetGroupType;

        // 运行时缓存
        Quaternion localRot;
        public Quaternion LocalRot => localRot;
        public void SetLocalRot(Quaternion localRot) => this.localRot = localRot;

        // 碰撞检测事件
        public CollisionEventHandler onTriggerEnter2D;
        public CollisionEventHandler onTriggerStay2D;
        public CollisionEventHandler onTriggerExit2D;
        public delegate void CollisionEventHandler(in CollisionEventModel args);

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
            if (!other.gameObject.TryGetComponent<EntityColliderModel>(out var otherColliderModel)) return;

            var otherFather = otherColliderModel.father;
            if (!IsInRightRelativeTargetGroup(otherFather)) return;

            CollisionEventModel args = new CollisionEventModel(father, otherFather, transform.position, other.transform.position);
            onTriggerEnter2D?.Invoke(args);
        }

        void OnTriggerStay2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<EntityColliderModel>(out var otherColliderModel)) return;
            var otherFather = otherColliderModel.father;
            if (!IsInRightRelativeTargetGroup(otherFather)) {
                return;
            }

            CollisionEventModel args = new CollisionEventModel(father, otherFather, transform.position, other.transform.position);
            onTriggerStay2D?.Invoke(args);
        }

        void OnTriggerExit2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<EntityColliderModel>(out var otherColliderModel)) return;
            var otherFather = otherColliderModel.father;
            if (!IsInRightRelativeTargetGroup(otherFather)) {
                return;
            }

            CollisionEventModel args = new CollisionEventModel(father, otherFather, transform.position, other.transform.position);
            onTriggerExit2D?.Invoke(args);
        }

        public bool IsInRightRelativeTargetGroup(in EntityIDArgs other) {
            var self = father;
            var selfAllyType = self.allyType;
            var otherAllyType = other.allyType;
            bool isSelf = self.IsTheSame(other);
            bool isAlly = selfAllyType.IsAlly(otherAllyType);
            bool isEnemy = selfAllyType.IsEnemy(otherAllyType);
            bool isOtherNeutral = otherAllyType == AllyType.Neutral;

            if (hitRelativeTargetGroupType == RelativeTargetGroupType.None)
                return false;

            if (hitRelativeTargetGroupType.Contains(RelativeTargetGroupType.Self) && isSelf)
                return true;

            if (hitRelativeTargetGroupType.Contains(RelativeTargetGroupType.Ally) && isAlly)
                return true;

            if (hitRelativeTargetGroupType.Contains(RelativeTargetGroupType.Enemy) && isEnemy)
                return true;

            if (hitRelativeTargetGroupType.Contains(RelativeTargetGroupType.Neutral) && isOtherNeutral)
                return true;

            return false;
        }

    }

}
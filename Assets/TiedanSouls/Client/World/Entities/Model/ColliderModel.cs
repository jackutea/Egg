using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 此脚本通过动态挂载
    /// </summary>
    public class ColliderModel : MonoBehaviour {

        IDArgs father;
        public IDArgs Father => father;

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
        public delegate void CollisionEventHandler(in CollisionEventArgs args);

        public bool IsActivated => gameObject.activeSelf;

        public void Activate() {
            gameObject.SetActive(true);
        }

        public void Deactivate() {
            gameObject.SetActive(false);
        }

        public void SetFather(IDArgs father) {
            this.father = father;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<ColliderModel>(out var otherColliderModel)) return;

            var otherFather = otherColliderModel.father;
            if (!IsRightHitTarget(otherFather)) return;

            CollisionEventArgs args = new CollisionEventArgs(father, otherFather);
            onTriggerEnter2D?.Invoke(args);
        }

        void OnTriggerStay2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<ColliderModel>(out var otherColliderModel)) return;

            var otherFather = otherColliderModel.father;
            if (!IsRightHitTarget(otherFather)) return;

            CollisionEventArgs args = new CollisionEventArgs(father, otherFather);
            onTriggerStay2D?.Invoke(args);
        }

        void OnTriggerExit2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<ColliderModel>(out var otherColliderModel)) return;

            var otherFather = otherColliderModel.father;
            if (!IsRightHitTarget(otherFather)) return;

            CollisionEventArgs args = new CollisionEventArgs(father, otherFather);
            onTriggerExit2D?.Invoke(args);
        }

        bool IsRightHitTarget(in IDArgs otherFather) {
            var selfFatherAllyType = father.allyType;
            var otherFatherAllyType = otherFather.allyType;
            bool isSelf = father.IsTheSame(otherFather);

            if (hitRelativeTargetGroupType == RelativeTargetGroupType.None) return false;
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.All) return true;
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.AllExceptSelf) return !isSelf;
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.AllExceptAlly) return !selfFatherAllyType.IsAlly(otherFatherAllyType);
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.AllExceptEnemy) return !selfFatherAllyType.IsEnemy(otherFatherAllyType);
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.AllExceptNeutral) return selfFatherAllyType != AllyType.Neutral;

            if (hitRelativeTargetGroupType == RelativeTargetGroupType.OnlySelf) return isSelf;
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.OnlyAlly) return selfFatherAllyType.IsAlly(otherFatherAllyType);
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.OnlyEnemy) return selfFatherAllyType.IsEnemy(otherFatherAllyType);
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.OnlyNeutral) return selfFatherAllyType == AllyType.Neutral;

            if (hitRelativeTargetGroupType == RelativeTargetGroupType.SelfAndAlly) return isSelf || selfFatherAllyType.IsAlly(otherFatherAllyType);
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.SelfAndEnemy) return isSelf || selfFatherAllyType.IsEnemy(otherFatherAllyType);
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.SelfAndNeutral) return isSelf || selfFatherAllyType == AllyType.Neutral;

            if (hitRelativeTargetGroupType == RelativeTargetGroupType.AllyAndEnemy) return selfFatherAllyType.IsAlly(otherFatherAllyType) || selfFatherAllyType.IsEnemy(otherFatherAllyType);
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.AllyAndNeutral) return selfFatherAllyType.IsAlly(otherFatherAllyType) || selfFatherAllyType == AllyType.Neutral;
            if (hitRelativeTargetGroupType == RelativeTargetGroupType.EnemyAndNeutral) return selfFatherAllyType.IsEnemy(otherFatherAllyType) || selfFatherAllyType == AllyType.Neutral;

            return false;
        }

    }

}
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
        TargetGroupType hitTargetGroupType;
        public void SetHitTargetGroupType(TargetGroupType hitTargetGroupType) => this.hitTargetGroupType = hitTargetGroupType;

        // 运行时缓存
        Quaternion localRot;
        public Quaternion LocalRot => localRot;
        public void SetLocalRot(Quaternion localRot) => this.localRot = localRot;

        // 碰撞检测事件
        public event CollisionEventHandler onTriggerEnter2D;
        public event CollisionEventHandler onTriggerStay2D;
        public event CollisionEventHandler onTriggerExit2D;
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

            if (hitTargetGroupType == TargetGroupType.None) return false;
            if (hitTargetGroupType == TargetGroupType.All) return true;
            if (hitTargetGroupType == TargetGroupType.AllExceptSelf) return !isSelf;
            if (hitTargetGroupType == TargetGroupType.AllExceptAlly) return !selfFatherAllyType.IsAlly(otherFatherAllyType);
            if (hitTargetGroupType == TargetGroupType.AllExceptEnemy) return !selfFatherAllyType.IsEnemy(otherFatherAllyType);
            if (hitTargetGroupType == TargetGroupType.AllExceptNeutral) return selfFatherAllyType != AllyType.Neutral;

            if (hitTargetGroupType == TargetGroupType.OnlySelf) return isSelf;
            if (hitTargetGroupType == TargetGroupType.OnlyAlly) return selfFatherAllyType.IsAlly(otherFatherAllyType);
            if (hitTargetGroupType == TargetGroupType.OnlyEnemy) return selfFatherAllyType.IsEnemy(otherFatherAllyType);
            if (hitTargetGroupType == TargetGroupType.OnlyNeutral) return selfFatherAllyType == AllyType.Neutral;

            if (hitTargetGroupType == TargetGroupType.SelfAndAlly) return isSelf || selfFatherAllyType.IsAlly(otherFatherAllyType);
            if (hitTargetGroupType == TargetGroupType.SelfAndEnemy) return isSelf || selfFatherAllyType.IsEnemy(otherFatherAllyType);
            if (hitTargetGroupType == TargetGroupType.SelfAndNeutral) return isSelf || selfFatherAllyType == AllyType.Neutral;

            if (hitTargetGroupType == TargetGroupType.AllyAndEnemy) return selfFatherAllyType.IsAlly(otherFatherAllyType) || selfFatherAllyType.IsEnemy(otherFatherAllyType);
            if (hitTargetGroupType == TargetGroupType.AllyAndNeutral) return selfFatherAllyType.IsAlly(otherFatherAllyType) || selfFatherAllyType == AllyType.Neutral;
            if (hitTargetGroupType == TargetGroupType.EnemyAndNeutral) return selfFatherAllyType.IsEnemy(otherFatherAllyType) || selfFatherAllyType == AllyType.Neutral;

            return false;
        }

    }

}
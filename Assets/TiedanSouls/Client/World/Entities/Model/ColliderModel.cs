using System;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 此脚本通过动态挂载到技能碰撞盒
    /// </summary>
    public class ColliderModel : MonoBehaviour {

        IDArgs father;
        public IDArgs Father => father;
        public void SetFather(IDArgs father) => this.father = father;

        public ColliderType colliderType;
        public Vector3 localPos;
        public float localAngleZ;
        public Vector3 size;

        public Quaternion localRot; // 运行时缓存

        public event CollisionEventHandler onTriggerEnter2D;
        public event CollisionEventHandler onTriggerStay2D;
        public event CollisionEventHandler onTriggerExit2D;
        public delegate void CollisionEventHandler(in CollisionEventArgs args);

        void OnTriggerEnter2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<ColliderModel>(out var model)) return;

            CollisionEventArgs args = new CollisionEventArgs(father, model.father);
            onTriggerEnter2D?.Invoke(args);
        }

        void OnTriggerStay2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<ColliderModel>(out var model)) return;

            CollisionEventArgs args = new CollisionEventArgs(father, model.father);
            onTriggerStay2D?.Invoke(args);
        }

        void OnTriggerExit2D(Collider2D other) {
            if (!other.gameObject.TryGetComponent<ColliderModel>(out var model)) return;

            CollisionEventArgs args = new CollisionEventArgs(father, model.father);
            onTriggerExit2D?.Invoke(args);
        }

    }

}
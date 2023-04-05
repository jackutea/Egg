using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public struct CollisionEventModel {

        public EntityCollider entityColliderModelA;
        public EntityCollider entityColliderModelB;
        public Vector3 normalA;

        public CollisionEventModel(EntityCollider a, EntityCollider b, Vector3 normalA) {
            this.entityColliderModelA = a;
            this.entityColliderModelB = b;
            this.normalA = normalA;
        }

        public override string ToString() {
            return $"A方-{entityColliderModelA.Father}\nB方-{entityColliderModelB.Father}";
        }

    }

}
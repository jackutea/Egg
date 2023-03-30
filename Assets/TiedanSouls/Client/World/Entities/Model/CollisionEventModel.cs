using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public struct CollisionEventModel {

        EntityIDArgs a;
        public EntityIDArgs A => a;

        EntityIDArgs b;
        public EntityIDArgs B => b;

        Vector3 posA;
        public Vector3 PosA => posA;

        Vector3 posB;
        public Vector3 PosB => posB;

        public CollisionEventModel(in EntityIDArgs a, in EntityIDArgs b, in Vector3 posA, in Vector3 posB) {
            this.a = a;
            this.b = b;
            this.posA = posA;
            this.posB = posB;
        }

        public override string ToString() {
            return $"A方-{a}\nB方-{b}";
        }

    }

}
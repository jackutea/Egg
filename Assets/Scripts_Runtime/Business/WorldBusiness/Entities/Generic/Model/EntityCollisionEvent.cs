using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public struct EntityCollisionEvent {

        public EntityCollider entityColliderModelA;
        public EntityCollider entityColliderModelB;
        public Vector3 normalA;
        public Vector3 normalB;

    }

}
using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    [Serializable]
    public struct ColliderModel {

        public ColliderType colliderType;
        public Vector2 localPos;
        public float angleZ;
        public Vector2 size;

    }

}
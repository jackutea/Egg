using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    [Serializable]
    public struct ColliderModel {

        public ColliderType colliderType;
        public Vector2 localPos;
        public float localAngleZ;
        public Vector2 size;

    }

}
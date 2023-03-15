using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    [Serializable]
    public struct ColliderModel {

        public ColliderType colliderType;
        public Vector3 localPos;
        public float localAngleZ;
        public Vector3 size;

        public Quaternion localRot; // 运行时缓存

    }

}
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 碰撞器模型
    /// </summary>
    public struct ColliderModel {

        public ColliderType colliderType;
        public Vector3 localPos;
        public float localAngleZ;
        public Vector3 localScale;

    }

}
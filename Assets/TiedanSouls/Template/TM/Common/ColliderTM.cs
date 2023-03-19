using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct ColliderTM {

        public ColliderType colliderType;
        public Vector3 localPos;
        public float localAngleZ;
        public Vector3 size;

    }

}
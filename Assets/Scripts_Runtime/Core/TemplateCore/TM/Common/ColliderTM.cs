using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct ColliderTM {

        public ColliderType colliderType;
        public Vector3 localPosition;
        public float localAngleZ;
        public Vector3 localScale;

    }

}
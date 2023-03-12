using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    public struct CollisionTriggerModel {

        public int startFrame;
        public int endFrame;

        public int triggerIntervalFrame;
        public int triggerMaintainFrame;

        public ColliderModel[] colliderArray;

    }

}
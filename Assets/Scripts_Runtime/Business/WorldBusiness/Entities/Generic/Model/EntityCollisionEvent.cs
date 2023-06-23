using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class EntityCollisionEvent {

        public EntityCollider entityColliderModelA;
        public EntityCollider entityColliderModelB;
        public Vector3 normalA;
        public Vector3 normalB;
        public ToggleState triggerState;
        public ToggleState lastTriggerState;

        public override string ToString() {
            return $"triggerState {triggerState} lastTriggerState {lastTriggerState}\nnormalA {normalA} normalB {normalB} A方-{entityColliderModelA.Father}\nB方-{entityColliderModelB.Father}";
        }

    }

}
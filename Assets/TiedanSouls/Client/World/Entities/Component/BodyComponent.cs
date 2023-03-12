using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class BodyComponent : MonoBehaviour {

        Collider2D coll;

        public event Action<Collider2D> OnBodyTriggerExitHandle;

        public void Ctor() {
            coll = GetComponent<Collider2D>();
        }

        public void SetTrigger(bool isTrigger) {
            coll.isTrigger = isTrigger;
        }

        void OnTriggerExit2D(Collider2D other) {
            OnBodyTriggerExitHandle?.Invoke(other);
        }

        public void Reset() {
        }

    }

}
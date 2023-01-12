using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class FootComponent : MonoBehaviour {

        Collider2D footColl;

        public event Action<Collision2D> OnCollisionEnterHandle;
        public event Action<Collision2D> OnCollisionExitHandle;

        public void Ctor() {
            footColl = GetComponent<Collider2D>();
        }

        void OnCollisionEnter2D(Collision2D other) {
            OnCollisionEnterHandle.Invoke(other);
        }

        void OnCollisionExit2D(Collision2D other) {
            OnCollisionExitHandle.Invoke(other);
        }

        public void SetTrigger(bool isTrigger) {
            footColl.isTrigger = isTrigger;
        }

    }

}
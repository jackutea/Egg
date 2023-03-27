using System;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class FootComponent : MonoBehaviour {

        Collider2D footCollider;

        public Action<Collider2D> footTriggerEnter;
        public Action<Collider2D> footTriggerExit;

        public void Ctor() {
            footCollider = GetComponent<Collider2D>();
        }

        public void Reset() {

        }

        void OnTriggerEnter2D(Collider2D other) {
            footTriggerEnter.Invoke(other);
        }
        void OnTriggerExit2D(Collider2D other) {
            footTriggerExit.Invoke(other);
        }

        public void TurnOnTrigger() {
            footCollider.isTrigger = true;
        }

        public void TurnOffTrigger() {
            footCollider.isTrigger = false;
        }

    }

}
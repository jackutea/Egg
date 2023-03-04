using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class FootComponent : MonoBehaviour {

        Collider2D footColl;

        public event Action<Collider2D> FootTriggerEnter;
        public event Action<Collider2D> FootTriggerExit;

        public void Ctor() {
            footColl = GetComponent<Collider2D>();
        }

        void OnTriggerEnter2D(Collider2D other) {
            FootTriggerEnter.Invoke(other);
        }
        void OnTriggerExit2D(Collider2D other) {
            FootTriggerExit.Invoke(other);
        }

        public void SetTrigger(bool isTrigger) {
            footColl.isTrigger = isTrigger;
        }

        public void Reset(){}

    }

}
using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class FootComponent : MonoBehaviour {

        public event Action<Collision2D> OnCollisionEnterHandle;

        void OnCollisionEnter2D(Collision2D other) {
            OnCollisionEnterHandle.Invoke(other);
        }

    }

}
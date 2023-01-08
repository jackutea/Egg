using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleEntity : MonoBehaviour {

        Transform body;
        Rigidbody2D rb;

        FootComponent footComponent;

        public event Action<RoleEntity, Collision2D> OnCollisionEnterHandle;

        public void Ctor() {

            body = transform.Find("body");
            rb = body.GetComponent<Rigidbody2D>();
            footComponent = body.GetComponentInChildren<FootComponent>();

            TDLog.Assert(rb != null);
            TDLog.Assert(footComponent != null);

            footComponent.OnCollisionEnterHandle += OnCollisionEnter;

        }

        public void SetPos(Vector2 pos) {
            body.position = pos;
        }

        void OnCollisionEnter(Collision2D other) {
            OnCollisionEnterHandle.Invoke(this, other);
        }

    }

}
using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class SkillorBoxElement : MonoBehaviour {

        Collider2D coll;
        Vector2 originPos;
        float originZAngle;

        public event Action<Collider2D> OnTriggerEnterHandle;

        public void FromTM(Template.SkillorBoxTM tm) {
            coll = tm.ToCollider2D(gameObject);
            originPos = tm.center;
            originZAngle = tm.zRotation;
            Deactive();
        }

        public void Deactive() {
            coll.enabled = false;
        }

        public void Active(Vector2 parentPos, float parentZAngle) {
            coll.enabled = true;
            transform.position = parentPos;
            transform.rotation = Quaternion.Euler(0, 0, originZAngle + parentZAngle);
        }

        void OnTriggerEnter2D(Collider2D other) {
            OnTriggerEnterHandle.Invoke(other);
        }

    }

}

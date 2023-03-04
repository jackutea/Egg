using System;
using UnityEngine;

namespace TiedanSouls.Generic {

    public class SkillorBoxElement : MonoBehaviour {

        Collider2D coll;
        Vector2 originPos;
        float originZAngle;

        public event Action<Collider2D> OnTriggerEnterHandle;

        public void Deactive() {
            coll.enabled = false;
        }

        public void Active(Vector2 parentPos, float parentZAngle, sbyte faceXDir) {
            coll.enabled = true;
            Vector2 pos = originPos;
            pos.x *= faceXDir;
            transform.position = parentPos + pos;
            transform.rotation = Quaternion.Euler(0, faceXDir < 0 ? 180 : 0, parentZAngle + originZAngle);
        }

        void OnTriggerEnter2D(Collider2D other) {
            OnTriggerEnterHandle.Invoke(other);
        }

    }

}

using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleEntity : MonoBehaviour {

        Transform body;
        Rigidbody2D rb;

        public void Ctor() {

            body = transform.Find("body");
            rb = body.GetComponent<Rigidbody2D>();

            TDLog.Assert(rb != null);

        }

        public void SetPos(Vector2 pos) {
            body.position = pos;
        }

    }

}
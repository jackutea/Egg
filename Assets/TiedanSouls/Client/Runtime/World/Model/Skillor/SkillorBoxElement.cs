using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class SkillorBoxElement {

        Collider2D collider;
        Vector2 originPos;
        float originZAngle;

        public SkillorBoxElement() {}

        public void FromTM(Template.SkillorBoxTM tm) {
            collider = tm.ToCollider2D();
            originPos = collider.transform.position;
            originZAngle = collider.transform.rotation.eulerAngles.z;
            Deactive();
        }

        public void Deactive() {
            collider.enabled = false;
        }

        public void Active(Vector2 parentPos, float parentZAngle) {
            collider.enabled = true;
            collider.transform.position = originPos + parentPos;
            collider.transform.rotation = Quaternion.Euler(0, 0, originZAngle + parentZAngle);
        }

    }

}

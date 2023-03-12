using UnityEngine;
using TiedanSouls.Template;

namespace TiedanSouls.SkillModifier.Editor {

    public class SkillBoxEditorGo : MonoBehaviour {

        public void Rename(int index) {
            gameObject.name = "b_" + index;
        }

        public SkillBoxTM ToTM() {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null) {
                return null;
            }

            var tm = new SkillBoxTM();
            if (collider is BoxCollider2D box) {
                tm.shapeType = SkillBoxShapeType.Rect;
                tm.center = box.offset;
                tm.size = box.size;
                tm.zRotation = collider.transform.eulerAngles.z;
                return tm;
            } else if (collider is CircleCollider2D circle) {
                tm.shapeType = SkillBoxShapeType.Circle;
                tm.center = circle.offset;
                tm.size = new Vector2(circle.radius, circle.radius);
                tm.zRotation = collider.transform.eulerAngles.z;
                return tm;
            } else {
                throw new System.Exception("Unknown collider type: " + collider.GetType());
            }
        }

    }
    
}
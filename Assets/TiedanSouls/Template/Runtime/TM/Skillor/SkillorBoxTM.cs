using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillorBoxTM {

        public SkillorBoxShapeType shapeType;
        public Vector2 center;
        public Vector2 size;
        public float zRotation;

        public SkillorBoxTM() {}

        public Collider2D ToCollider2D() {
            switch (shapeType) {
                case SkillorBoxShapeType.Rect:
                    var box = new GameObject("skillor_box_rect").AddComponent<BoxCollider2D>();
                    box.isTrigger = true;
                    box.size = size;
                    box.offset = center;
                    box.transform.Rotate(0, 0, zRotation);
                    return box;
                case SkillorBoxShapeType.Circle:
                    var circle = new GameObject("skillor_box_circle").AddComponent<CircleCollider2D>();
                    circle.isTrigger = true;
                    circle.radius = size.x / 2;
                    circle.offset = center;
                    return circle;
                default:
                    throw new Exception("Unknown shape type: " + shapeType);
            }
        }

    }

}
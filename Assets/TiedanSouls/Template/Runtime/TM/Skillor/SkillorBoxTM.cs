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

        public Collider2D ToCollider2D(GameObject go) {
            switch (shapeType) {
                case SkillorBoxShapeType.Rect:
                    var box = go.AddComponent<BoxCollider2D>();
                    box.isTrigger = true;
                    box.size = size;
                    box.offset = center;
                    box.transform.Rotate(0, 0, zRotation);
                    return box;
                case SkillorBoxShapeType.Circle:
                    var circle = go.AddComponent<CircleCollider2D>();
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
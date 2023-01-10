using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class SkillorFrameElement {

        object parentPtr;
        public SkillorModel Parent => parentPtr as SkillorModel;

        SkillorBoxElement[] boxes;

        public SkillorFrameElement(SkillorModel parent) {
            parentPtr = parent;
        }

        public void FromTM(Template.SkillorFrameTM tm) {
            if (tm.boxes != null) {
                var boxes = new SkillorBoxElement[tm.boxes.Length];
                for (int i = 0; i < boxes.Length; i += 1) {
                    boxes[i] = new GameObject("skillor_box").AddComponent<SkillorBoxElement>();
                    boxes[i].FromTM(tm.boxes[i]);
                    boxes[i].OnTriggerEnterHandle += Parent.OnEnterOther;
                }
                this.boxes = boxes;
            }
        }

        public void Deactive() {
            if (boxes != null) {
                for (int i = 0; i < boxes.Length; i += 1) {
                    boxes[i].Deactive();
                }
            }
        }

        public void Active(Vector2 parentPos, float parentZAngle) {
            if (boxes != null) {
                for (int i = 0; i < boxes.Length; i += 1) {
                    boxes[i].Active(parentPos, parentZAngle);
                }
            }
        }

    }

}

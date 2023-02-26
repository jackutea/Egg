using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class SkillorFrameElement {

        object parentPtr;
        public SkillorModel Parent => parentPtr as SkillorModel;

        // - Hit
        public HitPowerModel hitPower;

        // - Dash
        public bool hasDash;
        public Vector2 dashForce;
        public bool isDashEnd;

        // - Cancel
        public SkillorCancelModel[] cancels;

        SkillorBoxElement[] boxes;

        public SkillorFrameElement(SkillorModel parent) {
            parentPtr = parent;
        }

        public void FromTM(Template.SkillorFrameTM tm) {

            // - Hit
            if (tm.hitPower != null) {
                this.hitPower = new HitPowerModel();
                hitPower.FromTM(tm.hitPower);
            }

            // - Dash
            this.hasDash = tm.hasDash;
            this.dashForce = tm.dashForce;
            this.isDashEnd = tm.isDashEnd;

            // - Cancel
            var cancelTMArray = tm.cancelTMs;
            if (cancelTMArray != null) {
                var cancels = new SkillorCancelModel[cancelTMArray.Length];
                for (int i = 0; i < cancels.Length; i += 1) {
                    cancels[i] = new SkillorCancelModel();
                    cancels[i].FromTM(cancelTMArray[i]);
                }
                this.cancels = cancels;
            }

            // - Box
            var boxTMArray = tm.boxes;
            if (boxTMArray != null) {
                var boxes = new SkillorBoxElement[boxTMArray.Length];
                for (int i = 0; i < boxes.Length; i += 1) {
                    boxes[i] = new GameObject("skillor_box").AddComponent<SkillorBoxElement>();
                    boxes[i].FromTM(boxTMArray[i]);
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

        public void Active(Vector2 parentPos, float parentZAngle, sbyte faceXDir) {
            if (boxes != null) {
                for (int i = 0; i < boxes.Length; i += 1) {
                    boxes[i].Active(parentPos, parentZAngle, faceXDir);
                }
            }
        }

    }

}

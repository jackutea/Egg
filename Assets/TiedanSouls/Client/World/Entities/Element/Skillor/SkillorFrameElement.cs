using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillFrameElement {

        object parentPtr;
        public SkillModel Parent => parentPtr as SkillModel;

        // - Hit
        public HitPowerModel hitPower;

        // - Dash
        public bool hasDash;
        public Vector2 dashForce;
        public bool isDashEnd;

        // - Cancel
        public SkillCancelModel[] allCancelModels;

        SkillBoxElement[] boxes;

        public SkillFrameElement(SkillModel parent) {
            parentPtr = parent;
        }

        public void FromTM(Template.SkillFrameTM tm) {

            // - Hit
            if (tm.hitPower != null) {
                this.hitPower = new HitPowerModel();
                TM2ModelUtil.SetHitPowerModel(hitPower, tm.hitPower);
            }

            // - Dash
            this.hasDash = tm.hasDash;
            this.dashForce = tm.dashForce;
            this.isDashEnd = tm.isDashEnd;

            // - Cancel
            var cancelTMArray = tm.cancelTMs;
            if (cancelTMArray != null) {
                var cancelModels = new SkillCancelModel[cancelTMArray.Length];
                for (int i = 0; i < cancelModels.Length; i += 1) {
                    var cancelModel = new SkillCancelModel();
                    TM2ModelUtil.SetSkilloCancelModel(ref cancelModel, cancelTMArray[i]);
                    cancelModels[i] = cancelModel;
                }
                this.allCancelModels = cancelModels;
            }

            // - Box
            var boxTMArray = tm.boxes;
            if (boxTMArray != null) {
                var boxes = new SkillBoxElement[boxTMArray.Length];
                for (int i = 0; i < boxes.Length; i += 1) {
                    boxes[i] = new GameObject("skill_box").AddComponent<SkillBoxElement>();
                    boxes[i].FromTM(boxTMArray[i]);
                    boxes[i].OnTriggerEnterHandle += Parent.OnEnterOther;
                }
                this.boxes = boxes;
            }

        }

        public void DeactiveFrameBoxes() {
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

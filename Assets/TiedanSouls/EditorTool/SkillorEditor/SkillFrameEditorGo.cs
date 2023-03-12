using UnityEngine;
using TiedanSouls.Template;

namespace TiedanSouls.SkillModifier.Editor {

    public class SkillFrameEditorGo : MonoBehaviour {

        [SerializeField] HitPowerTM hitPower;

        [SerializeField] bool hasDash;
        [SerializeField] Vector2 dashForce;
        [SerializeField] bool isDashEnd;

        [SerializeField] SkillCancelTM[] cancels;

        public void Rename(int index) {
            gameObject.name = "f_" + index;
        }

        public SkillFrameTM ToTM() {

            var tm = new SkillFrameTM();

            // - Hit
            tm.hitPower = hitPower;
            
            // - Dash
            tm.hasDash = hasDash;
            tm.dashForce = dashForce;
            tm.isDashEnd = isDashEnd;

            // - Cancel
            tm.cancelTMs = cancels;

            // - Box
            var boxEditors = GetComponentsInChildren<SkillBoxEditorGo>();
            if (boxEditors.Length == 0) {
                return tm;
            }

            tm.boxes = new SkillBoxTM[boxEditors.Length];
            for (int i = 0; i < boxEditors.Length; i += 1) {
                var eBox = boxEditors[i];
                eBox.Rename(i);
                tm.boxes[i] = eBox.ToTM();
            }

            return tm;

        }

    }

}
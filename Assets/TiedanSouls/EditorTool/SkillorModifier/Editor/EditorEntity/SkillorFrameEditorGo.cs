using UnityEngine;
using TiedanSouls.Template;

namespace TiedanSouls.SkillorModifier.Editor {

    public class SkillorFrameEditorGo : MonoBehaviour {

        [SerializeField] HitPowerTM hitPower;

        [SerializeField] bool hasDash;
        [SerializeField] Vector2 dashForce;
        [SerializeField] bool isDashEnd;

        [SerializeField] SkillorCancelTM[] cancels;

        public void Rename(int index) {
            gameObject.name = "f_" + index;
        }

        public SkillorFrameTM ToTM() {

            var tm = new SkillorFrameTM();

            // - Hit
            tm.hitPower = hitPower;
            
            // - Dash
            tm.hasDash = hasDash;
            tm.dashForce = dashForce;
            tm.isDashEnd = isDashEnd;

            // - Cancel
            tm.cancels = cancels;

            // - Box
            var boxEditors = GetComponentsInChildren<SkillorBoxEditorGo>();
            if (boxEditors.Length == 0) {
                return tm;
            }

            tm.boxes = new SkillorBoxTM[boxEditors.Length];
            for (int i = 0; i < boxEditors.Length; i += 1) {
                var eBox = boxEditors[i];
                eBox.Rename(i);
                tm.boxes[i] = eBox.ToTM();
            }

            return tm;

        }

    }

}
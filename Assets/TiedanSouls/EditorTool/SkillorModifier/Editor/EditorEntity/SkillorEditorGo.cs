using UnityEngine;
using UnityEditor;
using TiedanSouls.Template;

namespace TiedanSouls.SkillorModifier.Editor {

    public class SkillorEditorGo : MonoBehaviour {

        [SerializeField] SkillorSo so;

        [SerializeField] int typeID;
        [SerializeField] string skillorName;
        [SerializeField] SkillorType skillorType;

        [SerializeField] AnimationClip weaponAnim;

        [ContextMenu("Save")]
        void Save() {

            var tm = new SkillorTM();
            so.tm = tm;

            tm.typeID = typeID;
            tm.name = skillorName;
            tm.skillorType = skillorType;
            tm.weaponAnimName = weaponAnim.name;

            var frameEditors = GetComponentsInChildren<SkillorFrameEditorGo>();
            if (frameEditors.Length == 0) {
                return;
            }

            tm.frames = new SkillorFrameTM[frameEditors.Length];
            for (int i = 0; i < frameEditors.Length; i += 1) {
                var eFrame = frameEditors[i];
                eFrame.Rename(i);
                tm.frames[i] = eFrame.ToTM();
            }

            EditorUtility.SetDirty(so);

        }

    }

}
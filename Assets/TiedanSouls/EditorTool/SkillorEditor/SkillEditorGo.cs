using UnityEngine;
using UnityEditor;
using TiedanSouls.Template;

namespace TiedanSouls.SkillModifier.Editor {

    public class SkillEditorGo : MonoBehaviour {

        [SerializeField] SkillSO so;

        // - Identity
        [SerializeField] int typeID;
        [SerializeField] string skillName;
        [SerializeField] SkillType skillType;

        // - Combo
        [SerializeField] int originalSkillTypeID;

        // - Frames (By GetComponentsInChildren<SkillFrameEditorGo>)

        // - Renderer
        [SerializeField] AnimationClip weaponAnim;

        [ContextMenu("Save")]
        void Save() {

            var tm = new SkillTM();
            so.tm = tm;

            // - Identity
            tm.typeID = typeID;
            tm.name = skillName;
            tm.skillType = skillType;

            // - Combo
            tm.originalSkillTypeID = originalSkillTypeID;

            // - Frames
            var frameEditors = GetComponentsInChildren<SkillFrameEditorGo>();
            if (frameEditors.Length == 0) {
                return;
            }

            tm.frames = new SkillFrameTM[frameEditors.Length];
            for (int i = 0; i < frameEditors.Length; i += 1) {
                var eFrame = frameEditors[i];
                eFrame.Rename(i);
                tm.frames[i] = eFrame.ToTM();
            }

            // - Renderer
            tm.weaponAnimName = weaponAnim.name;

            // - Save
            EditorUtility.SetDirty(so);

        }

    }

}
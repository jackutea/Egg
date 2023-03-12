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

        // - Renderer
        [SerializeField] AnimationClip weaponAnim;

        [ContextMenu("保存配置")]
        void Save() {

        }

    }

}
using UnityEngine;
using TiedanSouls.Template;

namespace TiedanSouls.EditorTool.SkillorEditor {

    public class SkillEditorGo : MonoBehaviour {

        [SerializeField] SkillSO so;

        // - Identity
        public int typeID;
        public string skillName;
        public SkillType skillType;

        // - Combo
        public int originalSkillTypeID;

        public HitPowerEM[] hitPowerEMs;

        // - Renderer
        [SerializeField] public AnimationClip weaponAnim;

        [ContextMenu("保存配置")]
        void Save() {
            var skillTM = EM2TMUtil.GetTM_Skill(this);
            so.tm = skillTM;
        }

    }

}
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

        [Header("打击力")] public HitPowerEM[] hitPowerEMArray;

        [Header("技能检测")] public CollisionTriggerEM[] colliderTriggerEMArray;

        // - Renderer
        [SerializeField] public AnimationClip weaponAnim;

        [ContextMenu("保存配置")]
        void SaveToSO() {
            var skillTM = EM2TMUtil.GetTM_Skill(this);
            so.tm = skillTM;
        }

    }

}
using UnityEngine;
using TiedanSouls.Template;
using UnityEditor;

namespace TiedanSouls.EditorTool.SkillEditor {

    public class SkillEditorGO : MonoBehaviour {

        public SkillSO so;

        public int typeID;
        public string skillName;
        public SkillType skillType;

        [Header("原始技能")] public int originSkillTypeID;

        [Header("连招技能名单")] public SkillCancelEM[] comboSkillCancelEMArray;
        [Header("可强制取消技能名单")] public SkillCancelEM[] cancelSkillCancelEMArray;
        [Header("打击力度(组)")] public HitPowerEM[] hitPowerEMArray;
        [Header("碰撞器(组)")] public CollisionTriggerEM[] colliderTriggerEMArray;

        [Header("武器动画文件")] public AnimationClip weaponAnimClip;

        [ContextMenu("保存配置")]
        public void Save() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }
            var skillTM = EM2TMUtil.GetTM_Skill(this);
            so.tm = skillTM;
        }

        public void Load() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            var skillTM = so.tm;
            this.typeID = skillTM.typeID;
            this.skillName = skillTM.skillName;
            this.skillType = skillTM.skillType;

            this.originSkillTypeID = skillTM.originSkillTypeID;

            this.comboSkillCancelEMArray = TM2EMUtil.GetEM_SkillCancel(skillTM.comboSkillCancelTMArray);
            this.cancelSkillCancelEMArray = TM2EMUtil.GetEM_SkillCancel(skillTM.cancelSkillCancelTMArray);
            this.hitPowerEMArray = TM2EMUtil.GetEMArray_HitPower(skillTM.hitPowerTMArray);
            this.colliderTriggerEMArray = TM2EMUtil.GetEMArray_CollisionTrigger(skillTM.collisionTriggerTMArray);

            this.weaponAnimClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(skillTM.weaponAnimClip_GUID));
        }

    }

}
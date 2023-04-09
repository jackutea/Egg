using UnityEngine;
using UnityEditor;
using GameArki.AddressableHelper;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    public class SkillEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public SkillSO so;

        [Header("类型ID")] public int typeID;
        [Header("技能名称")] public string skillName;
        [Header("技能持续帧")] public int maintainFrame;
        [Header("技能类型")] public SkillType skillType;
        [Header("原始技能")] public int originSkillTypeID;
        [Header("武器动画文件")] public AnimationClip weaponAnimClip;

        [Header("组合技名单 =================================== ")] public SkillCancelEM[] comboSkillCancelEMArray;
        [Header("连招技名单 =================================== ")] public SkillCancelEM[] cancelSkillCancelEMArray;
        [Header("技能效果器(组)")] public SkillEffectorEM[] skillEffectorEMArray;
        [Header("技能位移曲线(组)")] public SkillMoveCurveEM[] skillMoveCurveEMArray;
        [Header("碰撞器(组) ===================================")] public EntityColliderTriggerEM[] entityColliderTriggerEMArray;

        public void Save() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }
            var skillTM = EM2TMUtil.GetSkillTM(this);
            so.tm = skillTM;

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);

            var labelName = AssetLabelCollection.SO_SKILL;
            AddressableHelper.SetAddressable(so, labelName, labelName);
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

            this.maintainFrame = skillTM.maintainFrame;

            this.originSkillTypeID = skillTM.originSkillTypeID;

            this.comboSkillCancelEMArray = TM2EMUtil.GetSkillCancelEM(skillTM.comboSkillCancelTMArray);
            this.cancelSkillCancelEMArray = TM2EMUtil.GetSkillCancelEM(skillTM.cancelSkillCancelTMArray);
            this.entityColliderTriggerEMArray = TM2EMUtil.GetCollisionTriggerEMArray(skillTM.collisionTriggerTMArray);
            this.skillEffectorEMArray = TM2EMUtil.GetSkillEffectorEMArray(skillTM.skillEffectorTMArray);
            this.skillMoveCurveEMArray = TM2EMUtil.GetSkillMoveCurveEMArray(skillTM.skillMoveCurveTMArray);

            this.weaponAnimClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(skillTM.weaponAnimClip_GUID));
        }

    }

}
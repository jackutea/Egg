using UnityEngine;
using UnityEditor;
using GameArki.AddressableHelper;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    public class SkillEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public SkillSO so;

        [Header("技能ID")] public int typeID;
        [Header("原始技能ID")] public int originSkillTypeID;
        [Header("技能名称")] public string skillName;
        [Header("技能持续帧")] public int maintainFrame;
        [Header("技能按键")] public SkillCastKey castKey;
        [Header("技能类型")] public SkillType skillType;
        [Header("武器动画文件")] public AnimationClip weaponAnimClip;

        [Header("连招技名单     =================================== ")] public SkillCancelEM[] cancelSkillCancelEMArray;
        [Header("效果器组       =================================== ")] public EffectorTriggerEM[] effectorTriggerEMArray;
        [Header("角色召唤组     =================================== ")] public RoleSummonEM[] roleSummonEMArray;
        [Header("弹幕生成组     =================================== ")] public ProjectileCtorEM[] projectileCtorEMArray;
        [Header("Buff附加组     =================================== ")] public BuffAttachEM[] buffAttachEMArray;
        [Header("技能位移组     =================================== ")] public SkillMoveCurveEM[] skillMoveCurveEMArray;
        [Header("碰撞器组       =================================== ")] public EntityColliderTriggerEM[] entityColliderTriggerEMArray;

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

            this.cancelSkillCancelEMArray = TM2EMUtil.GetSkillCancelEM(skillTM.cancelSkillCancelTMArray);
            this.entityColliderTriggerEMArray = TM2EMUtil.GetCollisionTriggerEMArray(skillTM.hitToggleTMArray);
            this.effectorTriggerEMArray = TM2EMUtil.GetSkillEffectorEMArray(skillTM.effectorTriggerEMArray);
            this.skillMoveCurveEMArray = TM2EMUtil.GetSkillMoveCurveEMArray(skillTM.skillMoveCurveTMArray);

            this.weaponAnimClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(skillTM.weaponAnimClip_GUID));
        }

    }

}
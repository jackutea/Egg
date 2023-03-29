using TiedanSouls.Template;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    public static class TM2EMUtil {

        #region [Buff]

        public static void LoadToBuffEditorGO(BuffEditorGO editorGO, in BuffTM tm) {
            editorGO.typeID = tm.typeID;
            editorGO.buffName = tm.buffName;
            editorGO.description = tm.description;
            editorGO.icon = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(tm.iconGUID));

            editorGO.delayFrame = tm.delayFrame;
            editorGO.intervalFrame = tm.intervalFrame;
            editorGO.durationFrame = tm.durationFrame;
            editorGO.triggerTimes = tm.triggerTimes;

            editorGO.attributeEffectEM = GetAttributeEffectEM(tm.attributeEffectTM);
            editorGO.effectorTypeID = tm.effectorTypeID;
        }

        #endregion

        #region [AttributeEffect]

        public static AttributeEffectEM[] GetAttributeEffectEMArray(AttributeEffectTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            AttributeEffectEM[] emArray = new AttributeEffectEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetAttributeEffectEM(tmArray[i]);
            }
            return emArray;
        }

        public static AttributeEffectEM GetAttributeEffectEM(AttributeEffectTM tm) {
            AttributeEffectEM em;
            em.hpNCT = tm.hpNCT;
            em.hpEV = tm.hpEV;
            em.maxHpNCT = tm.maxHPNCT;
            em.maxHpEV = tm.maxHPEV;
            em.atkPowerNCT = tm.atkPowerNCT;
            em.atkPowerEV = tm.atkPowerEV;
            em.atkSpeedNCT = tm.atkSpeedNCT;
            em.atkSpeedEV = tm.atkSpeedEV;
            em.moveSpeedNCT = tm.moveSpeedNCT;
            em.moveSpeedEV = tm.moveSpeedEV;
            return em;
        }

        #endregion

        #region [ProjectileBullet]

        public static ProjectileBulletEM[] GetProjectileBulletEMArray(ProjectileBulletTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            ProjectileBulletEM[] emArray = new ProjectileBulletEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetProjectileBulletEM(tmArray[i]);
            }
            return emArray;
        }

        public static ProjectileBulletEM GetProjectileBulletEM(ProjectileBulletTM tm) {
            ProjectileBulletEM em;
            em.startFrame = tm.startFrame;
            em.endFrame = tm.endFrame;
            em.extraPenetrateCount = tm.extraPenetrateCount;
            em.localPos = GetVector3_Shrink100(tm.localPos_cm);
            em.localEulerAngles = tm.localEulerAngles;
            em.bulletTypeID = tm.bulletTypeID;
            return em;
        }

        #endregion

        #region [Skill]

        public static SkillMoveCurveEM[] GetSkillMoveCurveEMArray(SkillMoveCurveTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            SkillMoveCurveEM[] emArray = new SkillMoveCurveEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetSkillMoveCurveEM(tmArray[i]);
            }
            return emArray;
        }

        public static SkillMoveCurveEM GetSkillMoveCurveEM(SkillMoveCurveTM tm) {
            SkillMoveCurveEM em;
            em.startFrame = tm.startFrame;
            em.isFaceTo = tm.isFaceTo;
            em.moveCurveEM = GetMoveCurveEM(tm.moveCurveTM);
            return em;
        }

        public static SkillCancelEM[] GetSkillCancelEM(SkillCancelTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            SkillCancelEM[] emArray = new SkillCancelEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetSkillCancelEM(tmArray[i]);
            }
            return emArray;
        }

        public static SkillCancelEM GetSkillCancelEM(SkillCancelTM tm) {
            SkillCancelEM em;
            em.skillTypeID = tm.skillTypeID;
            em.startFrame = tm.startFrame;
            em.endFrame = tm.endFrame;
            return em;
        }

        public static SkillEffectorEM[] GetSkillEffectorEMArray(SkillEffectorTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            SkillEffectorEM[] emArray = new SkillEffectorEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetSkillEffectorEM(tmArray[i]);
            }
            return emArray;
        }

        public static SkillEffectorEM GetSkillEffectorEM(SkillEffectorTM tm) {
            SkillEffectorEM em;
            em.effectorTypeID = tm.effectorTypeID;
            em.triggerFrame = tm.triggerFrame;
            em.offsetPos = tm.offsetPos;
            return em;
        }

        #endregion

        #region [Effector]

        public static EffectorEM GetEffectorEM(EffectorTM tm) {
            EffectorEM em;
            em.typeID = tm.typeID;
            em.effectorName = tm.effectorName;
            em.entitySummonEMArray = GetEntitySummonEMArray(tm.entitySummonTMArray);
            em.entityDestroyEMArray = GetEntityDestroyEMArray(tm.entityDestroyTMArray);
            return em;
        }

        #endregion

        #region [CollisionTrigger]

        public static CollisionTriggerEM[] GetCollisionTriggerEMArray(CollisionTriggerTM[] tmArray) {
            if (tmArray == null) return null;
            CollisionTriggerEM[] emArray = new CollisionTriggerEM[tmArray.Length];
            for (int i = 0; i < tmArray.Length; i++) {
                emArray[i] = GetCollisionTriggerEM(tmArray[i]);
            }
            return emArray;
        }

        public static CollisionTriggerEM GetCollisionTriggerEM(CollisionTriggerTM tm) {
            CollisionTriggerEM em;

            em.isEnabled = tm.isEnabled;

            em.totalFrame = tm.totalFrame;
            em.delayFrame = tm.delayFrame;
            em.intervalFrame = tm.intervalFrame;
            em.maintainFrame = tm.maintainFrame;

            var editorTrans = Selection.activeGameObject.transform;
            var colliderRelativePathArray = tm.colliderRelativePathArray;
            var pathCount = colliderRelativePathArray.Length;
            GameObject[] colliderGOArray = new GameObject[pathCount];
            for (int i = 0; i < pathCount; i++) {
                var path = colliderRelativePathArray[i];
                var go = editorTrans.Find(path)?.gameObject;
                colliderGOArray[i] = go;
            }

            em.colliderGOArray = colliderGOArray;

            em.relativeTargetGroupType = tm.relativeTargetGroupType;
            em.damageEM = GetDamageEM(tm.damageTM);
            em.knockBackPowerEM = GetKnockBackEM(tm.knockBackPowerTM);
            em.knockUpPowerEM = GetKnockUpEM(tm.knockUpPowerTM);
            em.hitEffectorTypeID = tm.hitEffectorTypeID;

            return em;
        }

        #endregion

        #region [PhysicsPower]

        public static KnockBackEM GetKnockBackEM(this KnockBackTM tm) {
            KnockBackEM em;

            em.knockBackDisCurve = GetAnimationCurve(tm.knockBackDisCurve_KeyframeTMArray);
            em.knockBackDistance = GetFloat_Shrink100(tm.knockBackDistance_cm);
            em.knockBackCostFrame = tm.knockBackCostFrame;

            return em;
        }

        public static KnockUpEM GetKnockUpEM(this KnockUpTM tm) {
            KnockUpEM em;

            em.knockUpDisCurve = GetAnimationCurve(tm.knockUpDisCurve_KeyframeTMArray);
            em.knockUpHeight = GetFloat_Shrink100(tm.knockUpHeight_cm);
            em.knockUpCostFrame = tm.knockUpCostFrame;

            return em;
        }

        #endregion

        #region [Damage]

        public static DamageEM GetDamageEM(DamageTM tm) {
            DamageEM em;
            em.damageType = tm.damageType;
            em.damageBase = tm.damageBase;
            em.damageCurve = GetAnimationCurve(tm.damageCurve_KeyframeTMArray);
            return em;
        }

        #endregion

        #region [Keyframe]

        public static AnimationCurve GetAnimationCurve(KeyframeTM[] tmArray) {
            var keyframeArray = GetTMArray_Keyframe(tmArray);
            return new AnimationCurve(keyframeArray);
        }

        public static Keyframe[] GetTMArray_Keyframe(KeyframeTM[] tmArray) {
            if (tmArray == null) {
                Debug.LogWarning("KeyframeTM Array 为 null");
                return null;
            }
            var len = tmArray.Length;
            var keyframeArray = new Keyframe[len];
            for (int i = 0; i < len; i++) {
                var tm = tmArray[i];
                Keyframe keyframe = new Keyframe();
                keyframe.time = tm.time;
                keyframe.value = tm.value;
                keyframe.inTangent = tm.inTangent;
                keyframe.outTangent = tm.outTangent;
                keyframe.inWeight = tm.inWeight;
                keyframe.outWeight = tm.outWeight;
                keyframeArray[i] = keyframe;
            }
            return keyframeArray;
        }

        #endregion

        #region [EntitySummon]

        public static EntitySummonEM[] GetEntitySummonEMArray(EntitySummonTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntitySummonEM[] emArray = new EntitySummonEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetEffectorSummonEM(tmArray[i]);
            }
            return emArray;
        }

        public static EntitySummonEM GetEffectorSummonEM(EntitySummonTM tm) {
            EntitySummonEM em;
            em.entityType = tm.entityType;
            em.typeID = tm.typeID;
            em.controlType = tm.controlType;
            return em;
        }

        #endregion

        #region [EntityDestroy]

        public static EntityDestroyEM[] GetEntityDestroyEMArray(EntityDestroyTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntityDestroyEM[] emArray = new EntityDestroyEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetEntityDestroyEM(tmArray[i]);
            }
            return emArray;
        }

        public static EntityDestroyEM GetEntityDestroyEM(EntityDestroyTM tm) {
            EntityDestroyEM em;
            em.entityType = tm.entityType;
            em.relativeTargetGroupType = tm.relativeTargetGroupType;
            em.attributeSelector_IsEnabled = tm.attributeSelector_IsEnabled;
            em.attributeSelectorEM = GetAttributeSelectorEM(tm.attributeSelectorTM);
            return em;
        }

        #endregion

        #region [EntityTracking]

        public static EntityTrackEM GetEntityTrackEM(EntityTrackTM tm) {
            EntityTrackEM em;
            em.trackSpeed = GetFloat_Shrink100(tm.trackSpeed_cm);
            em.trackTargetGroupType = tm.trackTargetGroupType;
            em.entityTrackSelectorEM = GetEntityTrackSelectorEM(tm.entityTrackSelectorTM);
            return em;
        }

        public static EntityTrackSelectorEM GetEntityTrackSelectorEM(EntityTrackSelectorTM tm) {
            EntityTrackSelectorEM em;
            em.entityType = tm.entityType;
            em.isAttributeSelectorEnabled = tm.isAttributeSelectorEnabled;
            em.attributeSelectorEM = GetAttributeSelectorEM(tm.attributeSelectorTM);
            return em;
        }

        #endregion

        #region [Selector]

        public static AttributeSelectorEM[] GetAttributeSelectorEMArray(AttributeSelectorTM[] tmArray) {
            if (tmArray == null) return null;
            AttributeSelectorEM[] emArray = new AttributeSelectorEM[tmArray.Length];
            for (int i = 0; i < tmArray.Length; i++) {
                emArray[i] = GetAttributeSelectorEM(tmArray[i]);
            }
            return emArray;
        }

        public static AttributeSelectorEM GetAttributeSelectorEM(AttributeSelectorTM tm) {
            AttributeSelectorEM em;
            em.hp = tm.hp;
            em.hp_ComparisonType = tm.hp_ComparisonType;
            em.hpMax = tm.hpMax;
            em.hpMax_ComparisonType = tm.hpMax_ComparisonType;
            em.ep = tm.ep;
            em.ep_ComparisonType = tm.ep_ComparisonType;
            em.epMax = tm.epMax;
            em.epMax_ComparisonType = tm.epMax_ComparisonType;
            em.gp = tm.gp;
            em.gp_ComparisonType = tm.gp_ComparisonType;
            em.gpMax = tm.gpMax;
            em.gpMax_ComparisonType = tm.gpMax_ComparisonType;
            em.moveSpeed = tm.moveSpeed;
            em.moveSpeed_ComparisonType = tm.moveSpeed_ComparisonType;
            em.jumpSpeed = tm.jumpSpeed;
            em.jumpSpeed_ComparisonType = tm.jumpSpeed_ComparisonType;
            em.fallingAcceleration = tm.fallingAcceleration;
            em.fallingAcceleration_ComparisonType = tm.fallingAcceleration_ComparisonType;
            em.fallingSpeedMax = tm.fallingSpeedMax;
            em.fallingSpeedMax_ComparisonType = tm.fallingSpeedMax_ComparisonType;
            return em;
        }

        #endregion

        #region [MoveCurve]

        public static MoveCurveEM GetMoveCurveEM(MoveCurveTM tm) {
            MoveCurveEM em;
            em.moveDistance = GetFloat_Shrink100(tm.moveDistance_cm);
            em.moveTotalFrame = tm.moveTotalFrame;
            em.disCurve = GetAnimationCurve(tm.disCurve_KeyframeTMArray);
            return em;
        }

        #endregion

        #region [Misc]

        public static float GetFloat_Shrink100(int value) {
            return value / 100f;
        }

        public static Vector3 GetVector3_Shrink100(Vector3Int value) {
            return new Vector3(value.x / 100f, value.y / 100f, value.z / 100f);
        }

        #endregion

    }

}
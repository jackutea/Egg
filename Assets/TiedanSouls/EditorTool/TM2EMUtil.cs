using TiedanSouls.EditorTool.EffectorEditor;
using TiedanSouls.EditorTool.SkillEditor;
using TiedanSouls.Template;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    public static class TM2EMUtil {

        #region [Projectile]

        public static ProjectileElementEM[] GetEMArray_ProjectileElement(ProjectileElementTM[] tmArray) {
            if (tmArray == null) return null;
            ProjectileElementEM[] emArray = new ProjectileElementEM[tmArray.Length];
            for (int i = 0; i < tmArray.Length; i++) {
                emArray[i] = GetEM_ProjectileElement(tmArray[i]);
            }
            return emArray;
        }

        public static ProjectileElementEM GetEM_ProjectileElement(ProjectileElementTM tm) {
            ProjectileElementEM em;
            em.startFrame = tm.startFrame;
            em.endFrame = tm.endFrame;
            em.collisionTriggerEM = GetEM_CollisionTrigger(tm.collisionTriggerTM);
            em.hitEffectorEM = GetEM_Effector(tm.hitEffectorTM);
            em.deathEffectorEM = GetEM_Effector(tm.deathEffectorTM);
            em.extraHitTimes = tm.extraHitTimes;
            em.vfxPrefab = tm.vfxPrefab_GUID == null ? null : AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(tm.vfxPrefab_GUID));

            em.moveDistance_cm = tm.moveDistance;
            em.moveTotalFrame = tm.moveTotalFrame;
            em.moveCurve = GetAnimationCurve(tm.keyframeTMArray);

            em.relativeOffset_pos = tm.relativeOffset_pos;
            em.relativeOffset_euler = tm.relativeOffset_euler;

            return em;
        }

        #endregion

        #region [Skill]

        public static SkillCancelEM[] GetEM_SkillCancel(SkillCancelTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            SkillCancelEM[] emArray = new SkillCancelEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetEM_SkillCancel(tmArray[i]);
            }
            return emArray;
        }

        public static SkillCancelEM GetEM_SkillCancel(SkillCancelTM tm) {
            SkillCancelEM em;
            em.skillTypeID = tm.skillTypeID;
            em.startFrame = tm.startFrame;
            em.endFrame = tm.endFrame;
            return em;
        }

        #endregion

        #region [Effector]

        public static EffectorEM GetEM_Effector(EffectorTM tm) {
            EffectorEM em;
            em.typeID = tm.typeID;
            em.effectorName = tm.effectorName;
            em.entitySummonEMArray = GetEMArray_EntitySummon(tm.entitySummonTMArray);
            em.entityDestroyEMArray = GetEMArray_EntityDestroy(tm.entityDestroyTMArray);
            return em;
        }

        #endregion

        #region [CollisionTrigger]

        public static CollisionTriggerEM[] GetEMArray_CollisionTrigger(CollisionTriggerTM[] tmArray) {
            if (tmArray == null) return null;
            CollisionTriggerEM[] emArray = new CollisionTriggerEM[tmArray.Length];
            for (int i = 0; i < tmArray.Length; i++) {
                emArray[i] = GetEM_CollisionTrigger(tmArray[i]);
            }
            return emArray;
        }

        public static CollisionTriggerEM GetEM_CollisionTrigger(CollisionTriggerTM tm) {
            CollisionTriggerEM em;
            em.isEnabled = tm.isEnabled;
            em.startFrame = tm.startFrame;
            em.endFrame = tm.endFrame;
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
            em.hitPowerEM = GetEM_HitPower(tm.hitPowerTM);
            em.hitTargetGroupType = tm.hitTargetGroupType;

            return em;
        }


        #endregion

        #region [HitPower]

        public static HitPowerEM GetEM_HitPower(this HitPowerTM tm) {
            HitPowerEM em;

            // 伤害 曲线还原
            em.damageCurve = GetAnimationCurve(tm.damageCurve_KeyframeTMArray);
            em.damageBase = tm.damageBase;
            // 顿帧 曲线还原
            em.hitStunFrameCurve = GetAnimationCurve(tm.hitStunFrameCurve_KeyframeTMArray);
            em.hitStunFrameBase = tm.hitStunFrameBase;
            // 击退 曲线还原
            em.knockBackDisCurve = GetAnimationCurve(tm.knockBackDisCurve_KeyframeTMArray);
            em.knockBackDistance_cm = tm.knockBackDistance_cm;
            em.knockBackCostFrame = tm.knockBackCostFrame;
            // 击飞 曲线还原
            em.knockUpDisCurve = GetAnimationCurve(tm.knockUpDisCurve_KeyframeTMArray);
            em.knockUpHeight_cm = tm.knockUpCostFrame;
            em.knockUpCostFrame = tm.knockUpCostFrame;

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

        #region [Entity Summon]

        public static EntitySummonEM[] GetEMArray_EntitySummon(EntitySummonTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntitySummonEM[] emArray = new EntitySummonEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetEM_EffectorSummon(tmArray[i]);
            }
            return emArray;
        }

        public static EntitySummonEM GetEM_EffectorSummon(EntitySummonTM tm) {
            EntitySummonEM em;
            em.entityType = tm.entityType;
            em.typeID = tm.typeID;
            em.controlType = tm.controlType;
            return em;
        }

        #endregion

        #region [Entity Destroy]

        public static EntityDestroyEM[] GetEMArray_EntityDestroy(EntityDestroyTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntityDestroyEM[] emArray = new EntityDestroyEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetEM_EntityDestroy(tmArray[i]);
            }
            return emArray;
        }

        public static EntityDestroyEM GetEM_EntityDestroy(EntityDestroyTM tm) {
            EntityDestroyEM em;
            em.entityType = tm.entityType;
            em.targetGroupType = tm.targetGroupType;
            em.isEnabled_attributeSelector = tm.isEnabled_attributeSelector;
            em.attributeSelectorEM = GetEM_AttributeSelector(tm.attributeSelectorTM);
            return em;
        }

        #region [Selector]

        public static AttributeSelectorEM[] GetEMArray_AttributeSelector(AttributeSelectorTM[] tmArray) {
            if (tmArray == null) return null;
            AttributeSelectorEM[] emArray = new AttributeSelectorEM[tmArray.Length];
            for (int i = 0; i < tmArray.Length; i++) {
                emArray[i] = GetEM_AttributeSelector(tmArray[i]);
            }
            return emArray;
        }

        public static AttributeSelectorEM GetEM_AttributeSelector(AttributeSelectorTM tm) {
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

        #endregion

    }

}
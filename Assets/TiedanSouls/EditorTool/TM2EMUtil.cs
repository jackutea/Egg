using TiedanSouls.EditorTool.EffectorEditor;
using TiedanSouls.Template;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    public static class TM2EMUtil {

        #region [Bullet]

        public static BulletEM[] GetEMArray_Bullet(BulletTM[] tmArray) {
            var len = tmArray.Length;
            BulletEM[] emArray = new BulletEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetEM_Bullet(tmArray[i]);
            }
            return emArray;
        }

        public static BulletEM GetEM_Bullet(BulletTM tm) {
            BulletEM em;
            em.collisionTriggerEM = GetEM_CollisionTrigger(tm.collisionTriggerTM);
            em.hitEffectorEM = GetEM_Effector(tm.hitEffectorTM);
            em.deathEffectorEM = GetEM_Effector(tm.deathEffectorTM);

            var vfxGUI = tm.vfxPrefab_GUID;
            em.vfxPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(vfxGUI), typeof(GameObject)) as GameObject;

            em.moveDistance_cm = tm.moveDistance_cm;
            em.moveTotalFrame = tm.moveTotalFrame;
            em.disCurve = GetAnimationCurve(tm.disCurve_KeyframeTMArray);

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

            em.targetGroupType = tm.targetGroupType;
            em.damageEM = GetEM_Damage(tm.damageTM);
            em.knockBackPowerEM = GetEM_KnockBack(tm.knockBackPowerTM);
            em.knockUpPowerEM = GetEM_KnockUp(tm.knockUpPowerTM);
            em.hitEffectorEM = GetEM_Effector(tm.hitEffectorTM);
            em.stateEffectEM = GetEM_StateEffect(tm.stateEffectTM);

            return em;
        }


        #endregion

        #region [PhysicsPower]

        public static KnockBackEM GetEM_KnockBack(this KnockBackTM tm) {
            KnockBackEM em;

            em.knockBackDisCurve = GetAnimationCurve(tm.knockBackDisCurve_KeyframeTMArray);
            em.knockBackDistance_cm = tm.knockBackDistance_cm;
            em.knockBackCostFrame = tm.knockBackCostFrame;

            return em;
        }

        public static KnockUpEM GetEM_KnockUp(this KnockUpTM tm) {
            KnockUpEM em;

            em.knockUpDisCurve = GetAnimationCurve(tm.knockUpDisCurve_KeyframeTMArray);
            em.knockUpHeight_cm = tm.knockUpCostFrame;
            em.knockUpCostFrame = tm.knockUpCostFrame;

            return em;
        }

        #endregion

        #region [Damage]

        public static DamageEM GetEM_Damage(DamageTM tm) {
            DamageEM em;
            em.damageType = tm.damageType;
            em.damageBase = tm.damageBase;
            em.damageCurve = GetAnimationCurve(tm.damageCurve_KeyframeTMArray);
            return em;
        }

        #endregion

        #region [StateEffect]

        public static StateEffectEM GetEM_StateEffect(StateEffectTM tm) {
            StateEffectEM em;
            em.addStateFlag = tm.addStateFlag;
            em.effectStateValue = tm.effectStateValue;
            em.effectMaintainFrame = tm.effectMaintainFrame;
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
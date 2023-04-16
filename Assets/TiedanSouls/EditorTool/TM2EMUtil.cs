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
            editorGO.maxExtraStackCount = tm.maxExtraStackCount;

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
            em.hpEV = GetFloat_Shrink100(tm.hpEV_Expanded);
            em.hpNeedRevoke = tm.NeedRevokeHP;
            em.hpEffectTimes = tm.hpEffectTimes;

            em.hpMaxNCT = tm.hpMaxNCT;
            em.hpMaxEV = GetFloat_Shrink100(tm.hpMaxEV_Expanded);
            em.hpMaxNeedRevoke = tm.NeedRevokeHPMax;
            em.hpMaxEffectTimes = tm.hpMaxEffectTimes;

            em.moveSpeedNCT = tm.moveSpeedNCT;
            em.moveSpeedEV = GetFloat_Shrink100(tm.moveSpeedEV_Expanded);
            em.moveSpeedRevoke = tm.NeedRevokeMoveSpeed;
            em.moveSpeedEffectTimes = tm.moveSpeedEffectTimes;

            em.normalSkillSpeedBonusEV = GetFloat_Shrink100(tm.normalSkillSpeedBonusEV_Expanded);
            em.normalSkillSpeedBonusEffectTimes = tm.normalSkillSpeedBonusEffectTimes;
            em.normalSkillSpeedBonusNeedRevoke = tm.normalSkillSpeedBonusNeedRevoke;

            em.physicalDamageBonusEV = GetFloat_Shrink100(tm.physicalDamageBonusEV_Expanded);
            em.physicalDamageBonusEffectTimes = tm.physicalDamageBonusEffectTimes;
            em.physicalDamageBonusNeedRevoke = tm.physicalDamageBonusNeedRevoke;

            em.magicalDamageBonusEV = GetFloat_Shrink100(tm.magicalDamageBonusEV_Expanded);
            em.magicalDamageBonusEffectTimes = tm.magicalDamageBonusEffectTimes;
            em.magicalDamageBonusNeedRevoke = tm.magicalDamageBonusNeedRevoke;

            em.physicalDefenseBonusEV = GetFloat_Shrink100(tm.physicalDefenseBonusEV_Expanded);
            em.physicalDefenseBonusEffectTimes = tm.physicalDefenseBonusEffectTimes;
            em.physicalDefenseBonusNeedRevoke = tm.needRevokePhysicalDefenseBonus;

            em.magicalDefenseBonusEV = GetFloat_Shrink100(tm.magicalDefenseBonusEV_Expanded);
            em.magicalDefenseBonusEffectTimes = tm.magicalDefenseBonusEffectTimes;
            em.magicalDefenseBonusNeedRevoke = tm.magicalDefenseBonusNeedRevoke;

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
            em.triggerFrame = tm.triggerFrame;
            em.isFaceTo = tm.isFaceTo;
            em.needWaitForMoveEnd = tm.needWaitForMoveEnd;
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

        public static EffectorTriggerEM[] GetSkillEffectorEMArray(EffectorTriggerTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EffectorTriggerEM[] emArray = new EffectorTriggerEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetSkillEffectorEM(tmArray[i]);
            }
            return emArray;
        }

        public static EffectorTriggerEM GetSkillEffectorEM(EffectorTriggerTM tm) {
            EffectorTriggerEM em;
            em.effectorTypeID = tm.effectorTypeID;
            em.triggerFrame = tm.triggerFrame;
            return em;
        }

        #endregion

        #region [Effector]

        public static EffectorEM GetEffectorEM(EffectorTM tm) {
            EffectorEM em;
            em.typeID = tm.typeID;
            em.effectorName = tm.effectorName;
            em.roleSummonEMArray = GetRoleSummonEMArray(tm.roleSummonTMArray);
            em.buffAttachEMArray = GetBuffAttachEMArray(tm.buffAttachTMArray);
            em.projectileCtorEMArray = GetProjectileCtorEMArray(tm.projectileCtorTMArray);
            em.entityDestroyEMArray = GetEntityDestroyEMArray(tm.entityDestroyTMArray);
            return em;
        }

        #endregion

        #region [CollisionTrigger]

        public static EntityColliderTriggerEM[] GetCollisionTriggerEMArray(EntityColliderTriggerTM[] tmArray) {
            if (tmArray == null) return null;
            EntityColliderTriggerEM[] emArray = new EntityColliderTriggerEM[tmArray.Length];
            for (int i = 0; i < tmArray.Length; i++) {
                emArray[i] = GetCollisionTriggerEM(tmArray[i]);
            }
            return emArray;
        }

        public static EntityColliderTriggerEM GetCollisionTriggerEM(EntityColliderTriggerTM tm) {
            EntityColliderTriggerEM em;
            em.isEnabled = tm.isEnabled;
            em.frameRange = tm.frameRange;
            em.triggerMode = tm.triggerMode;
            em.triggerFixedIntervalEM = GetTriggerFixedIntervalEM(tm.triggerFixedIntervalTM);
            em.triggerCustomEM = GetTriggerCustomEM(tm.triggerCustomTM);

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

            em.targetEntityType = tm.targetEntityType;
            em.hitTargetGroupType = tm.hitTargetGroupType;

            em.damageEM = GetDamageEM(tm.damageTM);
            em.beHitEM = GetBeHitEM(tm.beHitTM);
            em.roleCtrlEffectEMArray = GetCtrlEffectEMArray(tm.roleCtrlEffectTMArray);
            em.hitEffectorTypeID = tm.hitEffectorTypeID;

            return em;
        }

        public static TriggerFixedIntervalEM GetTriggerFixedIntervalEM(TriggerFixedIntervalTM tm) {
            TriggerFixedIntervalEM em;
            em.delayFrame = tm.delayFrame;
            em.intervalFrame = tm.intervalFrame;
            em.maintainFrame = tm.maintainFrame;
            return em;
        }

        public static TriggerCustomEM GetTriggerCustomEM(TriggerCustomTM tm) {
            TriggerCustomEM em;
            em.frameRangeArray = tm.frameRangeArray.Clone() as Vector2Int[];
            return em;
        }

        #endregion

        #region [BeHit]

        public static BeHitEM GetBeHitEM(this BeHitTM tm) {
            BeHitEM em;

            em.beHitTotalFrame = tm.maintainFrame;

            em.knockBackTotalFrame = tm.knockBackTotalFrame;
            em.knockBackDisCurve = GetAnimationCurve(tm.knockBackKeyframeTMArray);
            em.knockBackDistance = GetFloat_Shrink100(tm.knockBackDistance_cm);
            em.knockBackTotalFrame = tm.knockBackTotalFrame;

            em.knockUpTotalFrame = tm.knockUpTotalFrame;
            em.knockUpDisCurve = GetAnimationCurve(tm.knockUpKeyframeTMArray);
            em.knockUpDis = GetFloat_Shrink100(tm.knockUpDis_cm);
            em.knockUpTotalFrame = tm.knockUpTotalFrame;

            return em;
        }

        #endregion

        #region [CtrlEffect]

        public static CtrlEffectEM[] GetCtrlEffectEMArray(RoleCtrlEffectTM[] tmArray) {
            if (tmArray == null) return null;
            CtrlEffectEM[] emArray = new CtrlEffectEM[tmArray.Length];
            for (int i = 0; i < tmArray.Length; i++) {
                emArray[i] = GetCtrlEffectEM(tmArray[i]);
            }
            return emArray;
        }

        public static CtrlEffectEM GetCtrlEffectEM(RoleCtrlEffectTM tm) {
            CtrlEffectEM em;
            em.ctrlEffectType = tm.roleCtrlEffectType;
            em.totalFrame = tm.totalFrame;
            em.icon = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(tm.iconGUID));
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

        #region [RoleSummon]

        public static RoleSummonEM[] GetRoleSummonEMArray(RoleSummonTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            RoleSummonEM[] emArray = new RoleSummonEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetRoleSummonEM(tmArray[i]);
            }
            return emArray;
        }

        public static RoleSummonEM GetRoleSummonEM(RoleSummonTM tm) {
            RoleSummonEM em;
            em.triggerFrame = tm.triggerFrame;
            em.typeID = tm.typeID;
            em.controlType = tm.controlType;
            em.localPos = GetVector3_Shrink100(tm.localPosExpanded);
            em.localEulerAngles = GetVector3_Shrink100(tm.localEulerAnglesExpanded);
            return em;
        }

        #endregion

        #region [BuffAttach]

        public static BuffAttachEM[] GetBuffAttachEMArray(BuffAttachTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            BuffAttachEM[] emArray = new BuffAttachEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetBuffAttachEM(tmArray[i]);
            }
            return emArray;
        }

        public static BuffAttachEM GetBuffAttachEM(BuffAttachTM tm) {
            BuffAttachEM em;
            em.triggerFrame = tm.triggerFrame;
            em.buffID = tm.buffID;
            return em;
        }

        #endregion

        #region [ProjectileCtor]

        public static ProjectileCtorEM[] GetProjectileCtorEMArray(ProjectileCtorTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            ProjectileCtorEM[] emArray = new ProjectileCtorEM[len];
            for (int i = 0; i < len; i++) {
                emArray[i] = GetProjectileCtorEM(tmArray[i]);
            }
            return emArray;
        }

        public static ProjectileCtorEM GetProjectileCtorEM(ProjectileCtorTM tm) {
            ProjectileCtorEM em;
            em.triggerFrame = tm.triggerFrame;
            em.typeID = tm.typeID;
            em.localPos = GetVector3_Shrink100(tm.localPosExpanded);
            em.localEulerAngles = GetVector3_Shrink100(tm.localEulerAnglesExpanded);
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
            em.hitTargetGroupType = tm.hitTargetGroupType;
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
            em.mp = tm.mp;
            em.ep_ComparisonType = tm.ep_ComparisonType;
            em.mpMax = tm.mpMax;
            em.mpMax_ComparisonType = tm.mpMax_ComparisonType;
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
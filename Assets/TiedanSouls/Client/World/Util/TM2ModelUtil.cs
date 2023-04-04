using System.Collections.Generic;
using UnityEngine;
using TiedanSouls.Generic;
using TiedanSouls.Template;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public static class TM2ModelUtil {

        #region [ProjectileBullet]

        public static ProjectileBulletModel[] GetProjectileBulletModelArray(ProjectileBulletTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            ProjectileBulletModel[] modelArray = new ProjectileBulletModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetProjectileBulletModel(tmArray[i]);
            }
            return modelArray;
        }

        public static ProjectileBulletModel GetProjectileBulletModel(ProjectileBulletTM tm) {
            ProjectileBulletModel model;
            model.startFrame = tm.startFrame;
            model.endFrame = tm.endFrame;
            model.bulletTypeID = tm.bulletTypeID;
            model.extraPenetrateCount = tm.extraPenetrateCount;
            model.localPos = GetVector3_Shrink100(tm.localPos_cm);
            model.localEulerAngles = tm.localEulerAngles;
            model.bulletEntityID = -1;
            return model;
        }

        #endregion

        #region [Skill]

        public static SkillMoveCurveModel[] GetSkillMoveCurveModelArray(SkillMoveCurveTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            SkillMoveCurveModel[] modelArray = new SkillMoveCurveModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetSkillMoveCurveModel(tmArray[i]);
            }
            return modelArray;
        }

        public static SkillMoveCurveModel GetSkillMoveCurveModel(SkillMoveCurveTM tm) {
            SkillMoveCurveModel model;
            model.startFrame = tm.startFrame;
            model.isFaceTo = tm.isFaceTo;
            model.moveCurveModel = GetMoveCurveModel(tm.moveCurveTM);
            return model;
        }

        public static SkillCancelModel[] GetSkillCancelModelArray(SkillCancelTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            SkillCancelModel[] modelArray = new SkillCancelModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetSkillCancelModel(tmArray[i]);
            }
            return modelArray;
        }

        public static SkillCancelModel GetSkillCancelModel(SkillCancelTM tm) {
            SkillCancelModel model;
            model.skillTypeID = tm.skillTypeID;
            model.startFrame = tm.startFrame;
            model.endFrame = tm.endFrame;
            return model;
        }

        public static SkillEffectorModel[] GetSkillEffectorModelArray(SkillEffectorTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            SkillEffectorModel[] modelArray = new SkillEffectorModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetSkillEffectorModel(tmArray[i]);
            }
            return modelArray;
        }

        public static SkillEffectorModel GetSkillEffectorModel(SkillEffectorTM tm) {
            SkillEffectorModel model;
            model.triggerFrame = tm.triggerFrame;
            model.effectorTypeID = tm.effectorTypeID;
            model.offsetPos = tm.offsetPos;
            return model;
        }

        #endregion

        #region [CollisionTrigger]

        public static CollisionTriggerModel[] GetCollisionTriggerModelArray(CollisionTriggerTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            CollisionTriggerModel[] modelArray = new CollisionTriggerModel[len];
            for (int i = 0; i < len; i++) {
                var tm = tmArray[i];
                modelArray[i] = GetCollisionTriggerModel(tm);
            }
            return modelArray;
        }

        public static CollisionTriggerModel GetCollisionTriggerModel(CollisionTriggerTM tm) {
            var frameRange = tm.frameRange;
            var totalFrame = frameRange.y - frameRange.x + 1;
            var triggerMode = tm.triggerMode;

            CollisionTriggerModel model;
            model.isEnabled = tm.isEnabled;
            model.triggerMode = triggerMode;
            model.triggerFixedIntervalModel = GetTriggerFixedIntervalModel(tm.triggerFixedIntervalTM);
            model.triggerCustomModel = GetTriggerCustomModel(tm.triggerCustomTM);

            model.targetEntityType = tm.targetEntityType;
            model.relativeTargetGroupType = tm.relativeTargetGroupType;

            model.damageModel = GetDamageModel(tm.damageTM);
            model.knockBackModel = GetKnockBackModel(tm.knockBackPowerTM);
            model.knockUpModel = GetKnockUpModel(tm.knockUpPowerTM);
            model.hitEffectorTypeID = tm.hitEffectorTypeID;
            model.colliderModelArray = GetColliderModelArray(tm.colliderTMArray, tm.relativeTargetGroupType);

            return model;
        }

        public static TriggerFixedIntervalModel GetTriggerFixedIntervalModel(TriggerFixedIntervalTM tm) {
            TriggerFixedIntervalModel model;
            model.delayFrame = tm.delayFrame;
            model.intervalFrame = tm.intervalFrame;
            model.maintainFrame = tm.maintainFrame;
            return model;
        }

        public static TriggerCustomModel GetTriggerCustomModel(TriggerCustomTM tm) {
            TriggerCustomModel model;

            Dictionary<int, TriggerState> triggerStatusDic = new Dictionary<int, TriggerState>();
            var frameRangeArray = tm.frameRangeArray;
            var len = frameRangeArray.Length;
            for (int i = 0; i < len; i++) {
                var frameRange = frameRangeArray[i];
                var startFrame = frameRange.x;
                var endFrame = frameRange.y;
                triggerStatusDic.Add(startFrame, TriggerState.Enter);
                triggerStatusDic.Add(endFrame, TriggerState.Exit);
                for (int j = startFrame + 1; j < endFrame; j++) {
                    triggerStatusDic.Add(j, TriggerState.Stay);
                }
            }
            model.triggerStateDic = triggerStatusDic;

            return model;
        }

        #endregion

        #region [Collider]

        public static ColliderModel[] GetColliderModelArray(ColliderTM[] tmArray, RelativeTargetGroupType hitRelativeTargetGroupType) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            ColliderModel[] modelArray = new ColliderModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetColliderModel(tmArray[i], hitRelativeTargetGroupType);
            }
            return modelArray;
        }

        public static ColliderModel GetColliderModel(ColliderTM tm, RelativeTargetGroupType hitRelativeTargetGroupType) {
            var go = GetGO_Collider(tm, true);
            ColliderModel model = go.AddComponent<ColliderModel>();
            model.SetColliderType(tm.colliderType);
            model.SetSize(tm.size);
            model.SetLocalPos(tm.localPos);
            model.SetLocalAngleZ(tm.localAngleZ);
            model.SetLocalRot(Quaternion.Euler(0, 0, tm.localAngleZ));
            model.SetHitRelativeTargetGroupType(hitRelativeTargetGroupType);
            return model;
        }

        public static GameObject GetGO_Collider(ColliderTM tm, bool isTrigger) {
            GameObject colliderGO = new GameObject();

            var colliderType = tm.colliderType;
            var colliderSize = tm.size;
            var localPos = tm.localPos;
            var localAngleZ = tm.localAngleZ;

            if (colliderType == ColliderType.Cube) {
                BoxCollider2D boxCollider = colliderGO.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = isTrigger;
                boxCollider.transform.localScale = new Vector2(colliderSize.x, colliderSize.y);
                boxCollider.transform.eulerAngles = new Vector3(0, 0, localAngleZ);
                colliderGO.name = "碰撞盒";
                colliderGO.SetActive(false);
            } else {
                TDLog.Error($"尚未支持的碰撞体类型: {colliderType}");
            }

            return colliderGO;
        }

        #endregion

        #region [KnockBack]

        public static KnockBackModel GetKnockBackModel(KnockBackTM tm) {
            KnockBackModel model;
            model.knockBackSpeedArray = GetFloatArray_Shrink100(tm.knockBackSpeedArray_cm);
            return model;
        }

        public static KnockUpModel GetKnockUpModel(KnockUpTM tm) {
            KnockUpModel model;
            model.knockUpSpeedArray = GetFloatArray_Shrink100(tm.knockUpSpeedArray_cm);
            return model;
        }

        #endregion

        #region [Damage]

        public static DamageModel GetDamageModel(DamageTM tm) {
            DamageModel model;
            model.damageType = tm.damageType;
            model.damageArray = tm.damageArray?.Clone() as int[];
            return model;
        }

        #endregion

        #region [AttributeEffect]

        public static AttributeEffectModel GetAttributeEffectModel(AttributeEffectTM tm) {
            AttributeEffectModel model;

            model.hpNCT = tm.hpNCT;
            model.hpEV = GetFloat_Shrink100(tm.hpEV_Expanded);
            model.hpEffectTimes = tm.hpEffectTimes;
            model.needRevoke_HPEV = tm.needRevokeHP;
            model.hpOffset= 0;

            model.hpMaxNCT = tm.hpMaxNCT;
            model.hpMaxEV = GetFloat_Shrink100(tm.hpMaxEV_Expanded);
            model.hpMaxEffectTimes = tm.hpMaxEffectTimes;
            model.needRevoke_HPMaxEV = tm.needRevokeHPMax;
            model.hpMaxOffset = 0;

            model.moveSpeedNCT = tm.moveSpeedNCT;
            model.moveSpeedEV = GetFloat_Shrink100(tm.moveSpeedEV_Expanded);
            model.moveSpeedEffectTimes = tm.moveSpeedEffectTimes;
            model.needRevoke_MoveSpeedEV = tm.needRevokeMoveSpeed;
            model.moveSpeedOffset = 0;

            model.physicalDamageBonusEV = GetFloat_Shrink100(tm.physicalDamageBonusEV_Expanded);
            model.physicalDamageBonusEffectTimes = tm.physicalDamageBonusEffectTimes;
            model.needRevokePhysicalDamageBonusEV = tm.needRevokePhysicalDamageBonus;
            model.physicalDamageBonusOffset = 0;

            model.magicalDamageBonusEV = GetFloat_Shrink100(tm.magicalDamageBonusEV_Expanded);
            model.magicalDamageBonusEffectTimes = tm.magicalDamageBonusEffectTimes;
            model.needRevokemagicalDamageBonus = tm.needRevokePhysicalDamageBonus;
            model.magicalDamageBonusOffset = 0;

            model.physicalDefenseBonusEV = GetFloat_Shrink100(tm.physicalDefenseBonusEV_Expanded);
            model.physicalDefenseBonusEffectTimes = tm.physicalDefenseBonusEffectTimes;
            model.needRevokePhysicalDefenseBonus = tm.needRevokePhysicalDefenseBonus;
            model.physicalDefenseBonusOffset = 0;

            model.magicalDefenseBonusEV = GetFloat_Shrink100(tm.magicalDefenseBonusEV_Expanded);
            model.magicalDefenseBonusEffectTimes = tm.magicalDefenseBonusEffectTimes;
            model.needRevokemagicalDefenseBonus = tm.needRevokemagicalDefenseBonus;
            model.magicalDefenseBonusOffset = 0;

            return model;
        }

        #endregion

        #region [Effector]

        public static EffectorModel GetEffectorModel(EffectorTM tm) {
            EffectorModel model;
            model.typeID = tm.typeID;
            model.effectorName = tm.effectorName;
            model.entitySummonModelArray = GetEntitySummonModelArray(tm.entitySummonTMArray);
            model.entityDestroyModelArray = GetEntityDestroyModelArray(tm.entityDestroyTMArray);
            return model;
        }

        #endregion

        #region [EntitySummon]

        public static EntitySummonModel[] GetEntitySummonModelArray(EntitySummonTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntitySummonModel[] modelArray = new EntitySummonModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetEntitySummonModel(tmArray[i]);
            }
            return modelArray;
        }

        public static EntitySummonModel GetEntitySummonModel(EntitySummonTM tm) {
            EntitySummonModel model;
            model.entityType = tm.entityType;
            model.typeID = tm.typeID;
            model.controlType = tm.controlType;
            return model;
        }

        #endregion

        #region [EntityDestroy]

        public static EntityDestroyModel[] GetEntityDestroyModelArray(EntityDestroyTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntityDestroyModel[] modelArray = new EntityDestroyModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetEntityDestroyModel(tmArray[i]);
            }
            return modelArray;
        }

        public static EntityDestroyModel GetEntityDestroyModel(EntityDestroyTM tm) {
            EntityDestroyModel model;
            model.entityType = tm.entityType;
            model.relativeTargetGroupType = tm.relativeTargetGroupType;
            model.isEnabled_attributeSelector = tm.attributeSelector_IsEnabled;
            model.attributeSelectorModel = GetAttributeSelectorModel(tm.attributeSelectorTM);
            return model;
        }

        #endregion

        #region [EntityTrack]

        public static EntityTrackModel GetEntityTrackModel(EntityTrackTM tm) {
            EntityTrackModel model;
            model.trackSpeed = GetFloat_Shrink100(tm.trackSpeed_cm);
            model.relativeTrackTargetGroupType = tm.trackTargetGroupType;
            model.entityTrackSelectorModel = GetEntityTrackSelectorModel(tm.entityTrackSelectorTM);
            model.target = default;
            return model;
        }

        public static EntityTrackSelectorModel GetEntityTrackSelectorModel(EntityTrackSelectorTM tm) {
            EntityTrackSelectorModel model;
            model.entityType = tm.entityType;
            model.isAttributeSelectorEnabled = tm.isAttributeSelectorEnabled;
            model.attributeSelectorModel = GetAttributeSelectorModel(tm.attributeSelectorTM);
            return model;
        }

        #endregion

        #region [MoveCurve]

        public static MoveCurveModel GetMoveCurveModel(MoveCurveTM tm) {
            MoveCurveModel model;
            model.moveSpeedArray = GetFloatArray_Shrink100(tm.moveSpeedArray);
            model.moveDirArray = GetVector3Array_Normalized(GetVector3Array_Shrink100(tm.moveDirArray));
            return model;
        }

        #endregion

        #region [Selector]

        public static AttributeSelectorModel[] GetAttributeSelectorModelArray(AttributeSelectorTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            AttributeSelectorModel[] modelArray = new AttributeSelectorModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetAttributeSelectorModel(tmArray[i]);
            }
            return modelArray;
        }

        public static AttributeSelectorModel GetAttributeSelectorModel(AttributeSelectorTM tm) {
            AttributeSelectorModel model;
            model.hp = tm.hp;
            model.hp_ComparisonType = tm.hp_ComparisonType;
            model.hpMax = tm.hpMax;
            model.hpMax_ComparisonType = tm.hpMax_ComparisonType;
            model.mp = tm.mp;
            model.ep_ComparisonType = tm.ep_ComparisonType;
            model.mpMax = tm.mpMax;
            model.mpMax_ComparisonType = tm.mpMax_ComparisonType;
            model.gp = tm.gp;
            model.gp_ComparisonType = tm.gp_ComparisonType;
            model.gpMax = tm.gpMax;
            model.gpMax_ComparisonType = tm.gpMax_ComparisonType;
            model.moveSpeed = tm.moveSpeed;
            model.moveSpeed_ComparisonType = tm.moveSpeed_ComparisonType;
            model.jumpSpeed = tm.jumpSpeed;
            model.jumpSpeed_ComparisonType = tm.jumpSpeed_ComparisonType;
            model.fallingAcceleration = tm.fallingAcceleration;
            model.fallingAcceleration_ComparisonType = tm.fallingAcceleration_ComparisonType;
            model.fallingSpeedMax = tm.fallingSpeedMax;
            model.fallingSpeedMax_ComparisonType = tm.fallingSpeedMax_ComparisonType;
            return model;
        }

        #endregion

        #region [MISC]

        public static Vector3[] GetVector3Array_Normalized(Vector3[] array) {
            if (array == null) return null;
            var len = array.Length;
            Vector3[] newArray = new Vector3[len];
            for (int i = 0; i < len; i++) {
                newArray[i] = array[i].normalized;
            }
            return newArray;
        }

        public static float[] GetFloatArray_Shrink100(int[] array) {
            if (array == null) return null;
            var len = array.Length;
            float[] newArray = new float[len];
            for (int i = 0; i < len; i++) {
                newArray[i] = array[i] * 0.01f;
            }
            return newArray;
        }

        public static float GetFloat_Shrink100(int v) {
            return v * 0.01f;
        }

        public static Vector3[] GetVector3Array_Shrink100(Vector3Int[] array) {
            if (array == null) return null;
            var len = array.Length;
            Vector3[] newArray = new Vector3[len];
            for (int i = 0; i < len; i++) {
                newArray[i] = GetVector3_Shrink100(array[i]);
            }
            return newArray;
        }

        public static Vector3 GetVector3_Shrink100(Vector3Int v) {
            return new Vector3(v.x * 0.01f, v.y * 0.01f, v.z * 0.01f);
        }

        #endregion

    }

}
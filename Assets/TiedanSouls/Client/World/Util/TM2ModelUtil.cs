using System.Collections.Generic;
using UnityEngine;
using TiedanSouls.Generic;
using TiedanSouls.Template;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public static class TM2ModelUtil {

        #region [Projectile]

        public static ProjectileElement[] GetElementArray_Projectile(ProjectileElementTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            ProjectileElement[] elementArray = new ProjectileElement[len];
            for (int i = 0; i < len; i++) {
                elementArray[i] = GetElement_Projectile(tmArray[i]);
            }
            return elementArray;
        }

        public static ProjectileElement GetElement_Projectile(ProjectileElementTM tm) {
            ProjectileElement element = new ProjectileElement();
            element.Ctor();
            element.SetStartFrame(tm.startFrame);
            element.SetEndFrame(tm.endFrame);
            element.SetCollisionTriggerModel(GetModel_CollisionTrigger(tm.collisionTriggerTM));
            element.SetHitEffectorModel(GetModel_Effector(tm.hitEffectorTM));
            element.SetDeathEffectorModel(GetModel_Effector(tm.deathEffectorTM));
            element.SetExtraHitTimes(tm.extraHitTimes);
            element.SetMoveSpeedArray(GetFloatArray_Shrink100(tm.moveSpeedArray_cm));
            element.SetDirectionArray(tm.directionArray.Clone() as Vector3[]);

            element.SetRelativeOffset_pos(GetVector3_Shrink100(tm.relativeOffset_pos));
            element.SetRelativeOffset_euler(tm.relativeOffset_euler);

            return element;
        }

        #endregion

        #region [Skill]

        public static SkillCancelModel[] GetModelArray_SkillCancel(SkillCancelTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            SkillCancelModel[] modelArray = new SkillCancelModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetModel_SkillCancel(tmArray[i]);
            }
            return modelArray;
        }

        public static SkillCancelModel GetModel_SkillCancel(SkillCancelTM tm) {
            SkillCancelModel model;
            model.skillTypeID = tm.skillTypeID;
            model.startFrame = tm.startFrame;
            model.endFrame = tm.endFrame;
            return model;
        }

        public static SkillEffectorModel[] GetModelArray_SkillEffector(SkillEffectorTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            SkillEffectorModel[] modelArray = new SkillEffectorModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetModel_SkillEffector(tmArray[i]);
            }
            return modelArray;
        }

        public static SkillEffectorModel GetModel_SkillEffector(SkillEffectorTM tm) {
            SkillEffectorModel model;
            model.triggerFrame = tm.triggerFrame;
            model.effectorTypeID = tm.effectorTypeID;
            model.offsetPos = tm.offsetPos;
            return model;
        }

        #endregion

        #region [CollisionTrigger]

        public static CollisionTriggerModel[] GetModelArray_CollisionTrigger(CollisionTriggerTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            CollisionTriggerModel[] modelArray = new CollisionTriggerModel[len];
            for (int i = 0; i < len; i++) modelArray[i] = GetModel_CollisionTrigger(tmArray[i]);
            return modelArray;
        }

        public static CollisionTriggerModel GetModel_CollisionTrigger(CollisionTriggerTM tm) {
            CollisionTriggerModel model;

            var startFrame = tm.startFrame;
            var endFrame = tm.endFrame;

            model.isEnabled = tm.isEnabled;
            model.startFrame = startFrame;
            model.endFrame = endFrame;

            model.triggerStatusDic = GetDic_TriggerStatus(startFrame, endFrame, tm.delayFrame, tm.intervalFrame, tm.maintainFrame);
            model.targetGroupType = tm.targetGroupType;

            model.damageModel = GetModel_Damage(tm.damageTM);
            model.knockBackPowerModel = GetModel_KnockBackPower(tm.knockBackPowerTM);
            model.knockUpPowerModel = GetModel_KnockUpPower(tm.knockUpPowerTM);
            model.hitEffectorModel = GetModel_Effector(tm.hitEffectorTM);
            model.stateEffectModel = GetModel_StateEffect(tm.stateEffectTM);

            model.colliderModelArray = GetModelArray_Collider(tm.colliderTMArray, tm.targetGroupType);

            return model;
        }

        public static Dictionary<int, TriggerStatus> GetDic_TriggerStatus(int startFrame,
                                                                          int endFrame,
                                                                          int delayFrame,
                                                                          int intervalFrame,
                                                                          int maintainFrame) {
            var dic = new Dictionary<int, TriggerStatus>();
            var T = intervalFrame + maintainFrame;
            for (int i = startFrame + delayFrame; i < endFrame; i += T) {
                dic.TryAdd(i, TriggerStatus.Begin);
                var end = i + intervalFrame;
                for (int j = i + 1; j < end; j++) {
                    dic.TryAdd(j, TriggerStatus.Triggering);
                }
                dic.TryAdd(end, TriggerStatus.End);
            }

            return dic;
        }


        #endregion

        #region [Collider]

        public static ColliderModel[] GetModelArray_Collider(ColliderTM[] tmArray, TargetGroupType hitTargetGroupType) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            ColliderModel[] modelArray = new ColliderModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetModel_Collider(tmArray[i], hitTargetGroupType);
            }
            return modelArray;
        }

        public static ColliderModel GetModel_Collider(ColliderTM tm, TargetGroupType hitTargetGroupType) {
            var go = GetGO_Collider(tm, true);
            ColliderModel model = go.AddComponent<ColliderModel>();
            model.SetColliderType(tm.colliderType);
            model.SetSize(tm.size);
            model.SetLocalPos(tm.localPos);
            model.SetLocalAngleZ(tm.localAngleZ);
            model.SetLocalRot(Quaternion.Euler(0, 0, tm.localAngleZ));
            model.SetHitTargetGroupType(hitTargetGroupType);
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
                colliderGO.name = $"技能碰撞盒_{colliderGO.GetInstanceID()}";
                colliderGO.SetActive(false);
            } else {
                TDLog.Error($"尚未支持的碰撞体类型: {colliderType}");
            }

            return colliderGO;
        }

        #endregion

        #region [KnockBackPower]

        public static KnockBackPowerModel GetModel_KnockBackPower(KnockBackPowerTM tm) {
            KnockBackPowerModel model;
            model.knockBackSpeedArray = GetFloatArray_Shrink100(tm.knockBackSpeedArray_cm);
            return model;
        }

        public static KnockUpPowerModel GetModel_KnockUpPower(KnockUpPowerTM tm) {
            KnockUpPowerModel model;
            model.knockUpSpeedArray = GetFloatArray_Shrink100(tm.knockUpSpeedArray_cm);
            return model;
        }

        #endregion

        #region [Damage]

        public static DamageModel GetModel_Damage(DamageTM tm) {
            DamageModel model;
            model.damageType = tm.damageType;
            model.damageArray = tm.damageArray.Clone() as int[];
            return model;
        }

        #endregion

        #region [StateEffect]

        public static StateEffectModel GetModel_StateEffect(StateEffectTM tm) {
            StateEffectModel model;
            model.effectStateType = tm.effectStateType;
            model.effectStateValue = tm.effectStateValue;
            model.effectMaintainFrame = tm.effectMaintainFrame;
            return model;
        }

        #endregion

        #region [Effector]

        public static EffectorModel GetModel_Effector(EffectorTM tm) {
            EffectorModel model;
            model.typeID = tm.typeID;
            model.effectorName = tm.effectorName;
            model.entitySummonModelArray = GetModelArray_EntitySummon(tm.entitySummonTMArray);
            model.entityDestroyModelArray = GetModelArray_EntityDestroy(tm.entityDestroyTMArray);
            return model;
        }

        #endregion

        #region [Entity Summon]

        public static EntitySummonModel[] GetModelArray_EntitySummon(EntitySummonTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntitySummonModel[] modelArray = new EntitySummonModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetModel_EntitySummon(tmArray[i]);
            }
            return modelArray;
        }

        public static EntitySummonModel GetModel_EntitySummon(EntitySummonTM tm) {
            EntitySummonModel model;
            model.entityType = tm.entityType;
            model.typeID = tm.typeID;
            model.controlType = tm.controlType;
            return model;
        }

        #endregion

        #region [Entity Destroy]

        public static EntityDestroyModel[] GetModelArray_EntityDestroy(EntityDestroyTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntityDestroyModel[] modelArray = new EntityDestroyModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetModel_EntityDestroy(tmArray[i]);
            }
            return modelArray;
        }

        public static EntityDestroyModel GetModel_EntityDestroy(EntityDestroyTM tm) {
            EntityDestroyModel model;
            model.entityType = tm.entityType;
            model.targetGroupType = tm.targetGroupType;
            model.isEnabled_attributeSelector = tm.isEnabled_attributeSelector;
            model.attributeSelectorModel = GetModel_AttributeSelector(tm.attributeSelectorTM);
            return model;
        }

        #endregion

        #region [Selector]

        public static AttributeSelectorModel[] GetModelArray_AttributeSelector(AttributeSelectorTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            AttributeSelectorModel[] modelArray = new AttributeSelectorModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetModel_AttributeSelector(tmArray[i]);
            }
            return modelArray;
        }

        public static AttributeSelectorModel GetModel_AttributeSelector(AttributeSelectorTM tm) {
            AttributeSelectorModel model;
            model.hp = tm.hp;
            model.hp_ComparisonType = tm.hp_ComparisonType;
            model.hpMax = tm.hpMax;
            model.hpMax_ComparisonType = tm.hpMax_ComparisonType;
            model.ep = tm.ep;
            model.ep_ComparisonType = tm.ep_ComparisonType;
            model.epMax = tm.epMax;
            model.epMax_ComparisonType = tm.epMax_ComparisonType;
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

        static float[] GetFloatArray_Shrink100(int[] array) {
            if (array == null) return null;
            var len = array.Length;
            float[] newArray = new float[len];
            for (int i = 0; i < len; i++) {
                newArray[i] = array[i] * 0.01f;
            }
            return newArray;
        }

        static Vector3 GetVector3_Shrink100(Vector3 v) {
            return new Vector3(v.x * 0.01f, v.y * 0.01f, v.z * 0.01f);
        }

        #endregion

    }

}
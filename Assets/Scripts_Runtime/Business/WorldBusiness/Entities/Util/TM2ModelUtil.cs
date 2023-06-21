using System.Collections.Generic;
using UnityEngine;
using TiedanSouls.Generic;
using TiedanSouls.Template;

namespace TiedanSouls.Client.Entities {

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
            model.triggerFrame = tm.startFrame;
            model.bulletTypeID = tm.bulletTypeID;
            model.localPos = GetVector3_Shrink100(tm.localPos_cm);
            model.localEulerAngles = tm.localEulerAngles;
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
            model.triggerFrame = tm.triggerFrame;
            model.isFaceTo = tm.isFaceTo;
            model.needWaitForMoveEnd = tm.needWaitForMoveEnd;
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

        public static EffectorTriggerModel[] GetSkillEffectorModelArray(EffectorTriggerTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EffectorTriggerModel[] modelArray = new EffectorTriggerModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetEffectorTriggerModel(tmArray[i]);
            }
            return modelArray;
        }

        public static EffectorTriggerModel GetEffectorTriggerModel(EffectorTriggerTM tm) {
            EffectorTriggerModel model;
            model.triggerFrame = tm.triggerFrame;
            model.effectorType = tm.effectorType;
            model.effectorTypeID = tm.effectorTypeID;
            return model;
        }

        #endregion

        #region [CollisionTrigger]

        public static EntityColliderTriggerModel[] GetEntityColliderTriggerModelArray(EntityColliderTriggerTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntityColliderTriggerModel[] modelArray = new EntityColliderTriggerModel[len];
            for (int i = 0; i < len; i++) {
                var tm = tmArray[i];
                modelArray[i] = GetEntityColliderTriggerModel(tm);
            }
            return modelArray;
        }

        public static EntityColliderTriggerModel GetEntityColliderTriggerModel(EntityColliderTriggerTM tm) {
            var frameRange = tm.frameRange;
            var totalFrame = frameRange.y - frameRange.x + 1;
            var triggerMode = tm.triggerMode;

            EntityColliderTriggerModel model;
            model.isEnabled = tm.isEnabled;
            model.triggerMode = triggerMode;
            model.triggerFixedIntervalModel = GetTriggerFixedIntervalModel(tm.triggerFixedIntervalTM);
            model.triggerCustomModel = GetTriggerCustomModel(tm.triggerCustomTM);

            model.targetEntityType = tm.targetEntityType;
            model.hitAllyType = tm.hitAllyType;

            model.damageModel = GetDamageModel(tm.damageTM);
            model.beHitModel = GetBeHitModel(tm.beHitTM);
            model.roleCtrlEffectModelArray = GetRoleCtrlEffectModelArray(tm.roleCtrlEffectTMArray);
            model.targetRoleEffectorTypeIDArray = tm.targetRoleEffectorTypeIDArray;
            model.selfRoleEffectorTypeIDArray = tm.selfRoleEffectorTypeIDArray;
            model.entityColliderArray = GetEntityColliderModelArray(tm.colliderTMArray, tm.hitAllyType);

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

        public static EntityCollider[] GetEntityColliderModelArray(ColliderTM[] tmArray, AllyType hitAllyType) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            EntityCollider[] modelArray = new EntityCollider[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetEntityColliderModel(tmArray[i], hitAllyType);
            }
            return modelArray;
        }

        public static EntityCollider GetEntityColliderModel(ColliderTM tm, AllyType hitAllyType) {
            var colliderModel = GetColliderModel(tm);
            var go = new GameObject("碰撞体");
            var colliderType = colliderModel.colliderType;
            if (colliderType == ColliderType.Cube) {
                var boxCollider = go.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = true;
                go.transform.localPosition = tm.localPosition;
                go.transform.localEulerAngles = new Vector3(0, 0, tm.localAngleZ);
                go.transform.localScale = tm.localScale;
            } else {
                TDLog.Error("未知的碰撞体类型");
            }
            EntityCollider model = go.AddComponent<EntityCollider>();
            model.SetColliderModel(colliderModel);
            model.SetHitTargetGroupType(hitAllyType);
            return model;
        }

        public static ColliderModel GetColliderModel(ColliderTM tm) {
            var go = GetGO_Collider(tm, true);
            ColliderModel model;
            model.colliderType = tm.colliderType;
            model.localPos = tm.localPosition;
            model.localAngleZ = tm.localAngleZ;
            model.localScale = tm.localScale;
            return model;
        }

        public static GameObject GetGO_Collider(ColliderTM tm, bool isTrigger) {
            GameObject colliderGO = new GameObject();

            var colliderType = tm.colliderType;
            var colliderSize = tm.localScale;
            var localPos = tm.localPosition;
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

        #region [Damage]

        public static DamageModel GetDamageModel(DamageTM tm) {
            DamageModel model;
            model.damageType = tm.damageType;
            model.damageArray = tm.damageArray?.Clone() as int[];
            return model;
        }

        #endregion

        #region [BeHit]

        public static BeHitModel GetBeHitModel(BeHitTM tm) {
            BeHitModel model;
            model.maintainFrame = tm.maintainFrame;
            model.knockBackSpeedArray = GetFloatArray_Shrink100(tm.knockBackSpeedArray_cm);
            model.knockUpSpeedArray = GetFloatArray_Shrink100(tm.knockUpSpeedArray_cm);
            return model;
        }

        #endregion

        #region [RoleCtrlEffect]

        public static RoleCtrlEffectModel[] GetRoleCtrlEffectModelArray(RoleCtrlEffectTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            RoleCtrlEffectModel[] modelArray = new RoleCtrlEffectModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetRoleCtrlEffectModel(tmArray[i]);
            }
            return modelArray;
        }

        public static RoleCtrlEffectModel GetRoleCtrlEffectModel(RoleCtrlEffectTM tm) {
            RoleCtrlEffectModel model;
            model.ctrlEffectType = tm.roleCtrlEffectType;
            model.totalFrame = tm.totalFrame;
            model.iconName = tm.iconName;
            return model;
        }

        #endregion

        #region [Modify]

        public static RoleModifyModel GetRoleModifyModel(RoleModifyTM tm) {
            RoleModifyModel model = new RoleModifyModel();

            model.hpNCT = tm.hpNCT;
            model.hpEV = GetFloat_Shrink100(tm.hpEV_Expanded);
            model.hpOffset = 0;

            model.hpMaxNCT = tm.hpMaxNCT;
            model.hpMaxEV = GetFloat_Shrink100(tm.hpMaxEV_Expanded);
            model.hpMaxOffset = 0;

            model.moveSpeedNCT = tm.moveSpeedNCT;
            model.moveSpeedEV = GetFloat_Shrink100(tm.moveSpeedEV_Expanded);
            model.moveSpeedOffset = 0;

            model.normalSkillSpeedBonusEV = GetFloat_Shrink100(tm.normalSkillSpeedBonusEV_Expanded);
            model.normalSkillSpeedBonusOffset = 0;

            model.physicalDamageBonusEV = GetFloat_Shrink100(tm.physicalDamageBonusEV_Expanded);
            model.physicalDamageBonusOffset = 0;

            model.magicalDamageBonusEV = GetFloat_Shrink100(tm.magicalDamageBonusEV_Expanded);
            model.magicalDamageBonusOffset = 0;

            model.physicalDefenseBonusEV = GetFloat_Shrink100(tm.physicalDefenseBonusEV_Expanded);
            model.physicalDefenseBonusOffset = 0;

            model.magicalDefenseBonusEV = GetFloat_Shrink100(tm.magicalDefenseBonusEV_Expanded);
            model.magicalDefenseBonusOffset = 0;

            return model;
        }

        public static SkillModifyModel GetSkillModifyModel(SkillModifyTM tm) {
            SkillModifyModel model;
            model.cdTime_EV = GetFloat_Shrink100(tm.cdTime_EV_Expanded);
            model.cdTime_NCT = tm.cdTime_NCT;
            return model;
        }

        #endregion

        #region [Effector]

        public static EffectorEntity GetRoleEffectorEntity(EffectorTM tm) {
            EffectorEntity model = new EffectorEntity();
            model.typeID = tm.typeID;
            model.effectorName = tm.effectorName;
            model.roleEffectorModel = GetRoleEffectorModel(tm.roleEffectorTM);
            model.skillEffectorModel = GetSkillEffectorModel(tm.skillEffectorTM);
            return model;
        }

        public static RoleEffectorModel GetRoleEffectorModel(RoleEffectorTM tm) {
            RoleEffectorModel model = new RoleEffectorModel();
            model.roleSelectorModel = GetRoleSelectorModel(tm.roleSelectorTM);
            model.roleModifyModel = GetRoleModifyModel(tm.roleModifyTM);
            return model;
        }

        public static SkillEffectorModel GetSkillEffectorModel(SkillEffectorTM tm) {
            SkillEffectorModel model = new SkillEffectorModel();
            model.skillSelectorModel = GetSkillSelectorModel(tm.skillSelectorTM);
            model.skillModifyModel = GetSkillModifyModel(tm.skillModifyTM);
            return model;
        }

        #endregion

        #region [RoleSummon]

        public static RoleSummonModel[] GetRoleSummonModelArray(RoleSummonTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            RoleSummonModel[] modelArray = new RoleSummonModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetRoleSummonModel(tmArray[i]);
            }
            return modelArray;
        }

        public static RoleSummonModel GetRoleSummonModel(RoleSummonTM tm) {
            RoleSummonModel model;
            model.triggerFrame = tm.triggerFrame;
            model.typeID = tm.typeID;
            model.controlType = tm.controlType;
            model.localPos = GetVector3_Shrink100(tm.localPosExpanded);
            model.localEulerAngles = GetVector3_Shrink100(tm.localEulerAnglesExpanded);
            return model;
        }

        #endregion

        #region [BuffAttach]

        public static BuffAttachModel[] GetBuffAttachModelArray(BuffAttachTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            BuffAttachModel[] modelArray = new BuffAttachModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetBuffAttachModel(tmArray[i]);
            }
            return modelArray;
        }


        public static BuffAttachModel GetBuffAttachModel(BuffAttachTM tm) {
            BuffAttachModel model;
            model.buffID = tm.buffID;
            model.triggerFrame = tm.triggerFrame;
            return model;
        }

        #endregion

        #region [ProjectileCtor]

        public static ProjectileCtorModel[] GetProjectileCtorModelArray(ProjectileCtorTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            ProjectileCtorModel[] modelArray = new ProjectileCtorModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetProjectileCtorModel(tmArray[i]);
            }
            return modelArray;
        }

        public static ProjectileCtorModel GetProjectileCtorModel(ProjectileCtorTM tm) {
            ProjectileCtorModel model;
            model.triggerFrame = tm.triggerFrame;
            model.typeID = tm.typeID;
            model.localPos = GetVector3_Shrink100(tm.localPosExpanded);
            model.localEulerAngles = GetVector3_Shrink100(tm.localEulerAnglesExpanded);
            return model;
        }

        #endregion

        #region [Field]

        public static FieldDoorModel[] GetFieldDoorModelArray(FieldDoorTM[] tmArray) {
            var len = tmArray.Length;
            var modelArray = new FieldDoorModel[len];
            for (int i = 0; i < len; i++) {
                var tm = tmArray[i];
                var model = GetFieldDoorModel(tm);
                modelArray[i] = model;
            }
            return modelArray;
        }

        public static FieldDoorModel GetFieldDoorModel(FieldDoorTM tm) {
            FieldDoorModel model;
            model.doorIndex = tm.doorIndex;
            model.fieldTypeID = tm.fieldTypeID;
            model.pos = GetVector3_Shrink100(tm.pos_cm);
            return model;
        }

        public static FieldSpawnEntityCtrlModel[] GetEntitySpawnCtrlModelArray(FieldSpawnEntityCtrlTM[] tmArray) {
            var len = tmArray.Length;
            var modelArray = new FieldSpawnEntityCtrlModel[len];
            for (int i = 0; i < len; i++) {
                var tm = tmArray[i];
                var model = GetEntitySpawnCtrlModel(tm);
                modelArray[i] = model;
            }
            return modelArray;
        }

        public static FieldSpawnEntityCtrlModel GetEntitySpawnCtrlModel(FieldSpawnEntityCtrlTM tm) {
            FieldSpawnEntityCtrlModel model;
            model.spawnFrame = tm.spawnFrame;
            model.isBreakPoint = tm.isBreakPoint;
            model.entitySpawnModel = GetEntitySpawnModel(tm.entitySpawnTM);
            return model;
        }

        #endregion

        #region [EntitySpawn]

        public static EntitySpawnModel GetEntitySpawnModel(EntitySpawnTM tm) {
            EntitySpawnModel model;
            model.entityType = tm.entityType;
            model.typeID = tm.typeID;
            model.controlType = tm.controlType;
            model.campType = tm.campType;
            model.spawnPos = tm.spawnPos;
            model.isBoss = tm.isBoss;
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
            model.attributeSelectorModel = GetRoleSelectorModel(tm.attributeSelectorTM);
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

        public static RoleSelectorModel[] GetAttributeSelectorModelArray(RoleSelectorTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            RoleSelectorModel[] modelArray = new RoleSelectorModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetRoleSelectorModel(tmArray[i]);
            }
            return modelArray;
        }

        public static RoleSelectorModel GetRoleSelectorModel(RoleSelectorTM tm) {
            RoleSelectorModel model;

            model.hp = tm.hp;
            model.hp_ComparisonType = tm.hp_ComparisonType;

            model.hpMax = tm.hpMax;
            model.hpMax_ComparisonType = tm.hpMax_ComparisonType;

            model.mp = tm.mp;
            model.mp_ComparisonType = tm.mp_ComparisonType;
            model.mpMax = tm.mpMax;
            model.mpMax_ComparisonType = tm.mpMax_ComparisonType;

            model.gp = tm.gp;
            model.gp_ComparisonType = tm.gp_ComparisonType;
            model.gpMax = tm.gpMax;
            model.gpMax_ComparisonType = tm.gpMax_ComparisonType;

            return model;
        }

        public static SkillSelectorModel GetSkillSelectorModel(SkillSelectorTM tm) {
            SkillSelectorModel model;
            model.skillTypeFlag = tm.skillTypeFlag;
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
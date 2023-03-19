using UnityEngine;
using TiedanSouls.Generic;
using TiedanSouls.Template;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public static class TM2ModelUtil {

        #region 

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

        #endregion

        #region [CollisionTrigger]

        public static CollisionTriggerModel[] GetModelArray_CollisionTrigger(CollisionTriggerTM[] tmArray) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            CollisionTriggerModel[] modelArray = new CollisionTriggerModel[len];
            for (int i = 0; i < len; i++) {
                var tm = tmArray[i];
                var startFrame = tm.startFrame;
                var endFrame = tm.endFrame;

                CollisionTriggerModel model;

                model.startFrame = startFrame;
                model.endFrame = endFrame;
                model.delayFrame = tm.delayFrame;
                model.intervalFrame = tm.intervalFrame;
                model.maintainFrame = tm.maintainFrame;

                model.colliderModelArray = GetModelArray_Collider(tm.colliderTMArray, tm.hitTargetType);
                model.hitPower = GetModel_HitPower(tm.hitPowerTM, startFrame, endFrame);
                modelArray[i] = model;
            }
            return modelArray;
        }

        #endregion

        #region [Collider]

        public static ColliderModel[] GetModelArray_Collider(ColliderTM[] tmArray, TargetType hitTargetType) {
            if (tmArray == null) return null;
            var len = tmArray.Length;
            ColliderModel[] modelArray = new ColliderModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetModel_Collider(tmArray[i], hitTargetType);
            }
            return modelArray;
        }

        public static ColliderModel GetModel_Collider(ColliderTM tm, TargetType hitTargetType) {
            var go = GetGO_Collider(tm, true);
            ColliderModel model = go.AddComponent<ColliderModel>();
            model.SetColliderType(tm.colliderType);
            model.SetSize(tm.size);
            model.SetLocalPos(tm.localPos);
            model.SetLocalAngleZ(tm.localAngleZ);
            model.SetLocalRot(Quaternion.Euler(0, 0, tm.localAngleZ));
            model.SetHitTargetType(hitTargetType);
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

        #region [HitPower]

        public static HitPowerModel GetModel_HitPower(HitPowerTM tm, int startFrame, int endFrame) {
            HitPowerModel model = new HitPowerModel();
            model.SetHitStunFrameArray(tm.hitStunFrameArray?.Clone() as int[]);
            model.SetDamageArray(tm.damageArray?.Clone() as int[]);
            model.SetKnockBackVelocityArray(GetFloatArray_Shrink100(tm.knockBackSpeedArray_cm));
            model.SetKnockUpVelocityArray(GetFloatArray_Shrink100(tm.knockUpSpeedArray_cm));
            model.startFrame = startFrame;
            model.endFrame = endFrame;
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

        #endregion

    }

}
using TiedanSouls.Generic;
using TiedanSouls.Template;
using UnityEngine;

namespace TiedanSouls {

    public static class TM2ModelUtil {


        #region [CollisionTrigger]

        public static CollisionTriggerModel[] GetModelArray_CollisionTrigger(CollisionTriggerTM[] tmArray) {
            var len = tmArray.Length;
            CollisionTriggerModel[] modelArray = new CollisionTriggerModel[len];
            for (int i = 0; i < len; i++) {
                var tm = tmArray[i];
                CollisionTriggerModel model;
                model.startFrame = tm.startFrame;
                model.endFrame = tm.endFrame;
                model.triggerIntervalFrame = tm.triggerIntervalFrame;
                model.triggerMaintainFrame = tm.triggerMaintainFrame;
                model.colliderArray = GetModelArray_Collider(tm.colliderTMArray);
                modelArray[i] = model;
            }
            return modelArray;
        }

        #endregion

        #region [Collider]

        public static ColliderModel[] GetModelArray_Collider(ColliderTM[] tmArray) {
            var len = tmArray.Length;
            ColliderModel[] modelArray = new ColliderModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetModel_Collider(tmArray[i]);
            }
            return modelArray;
        }

        public static ColliderModel GetModel_Collider(ColliderTM tm) {
            ColliderModel model;
            model.colliderType = tm.colliderType;
            model.localPos = tm.localPos;
            model.angleZ = tm.angleZ;
            model.size = tm.size;
            return model;
        }

        #endregion

        #region [HitPower]

        public static HitPowerModel[] GetModelArray_HitPower(HitPowerTM[] tmArray) {
            var len = tmArray.Length;
            HitPowerModel[] modelArray = new HitPowerModel[len];
            for (int i = 0; i < len; i++) {
                modelArray[i] = GetModel_HitPower(tmArray[i]);
            }
            return modelArray;
        }

        public static HitPowerModel GetModel_HitPower(HitPowerTM tm) {
            HitPowerModel model;
            model.startFrame = tm.startFrame;
            model.endFrame = tm.endFrame;
            model.hitStunFrameArray = tm.hitStunFrameArray;
            model.damageArray = tm.damageArray;
            model.knockBackVelocityArray = GetFloatArray_Shrink100(tm.knockBackSpeedArray_cm);
            model.knockUpVelocityArray = GetFloatArray_Shrink100(tm.knockUpSpeedArray_cm);
            return model;
        }

        #endregion

        #region [MISC]

        static float[] GetFloatArray_Shrink100(int[] array) {
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
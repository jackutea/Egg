using TiedanSouls.Template;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    public static class TM2EMUtil {

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
            SkillCancelEM em = new SkillCancelEM();
            em.skillTypeID = tm.skillTypeID;
            em.startFrame = tm.startFrame;
            em.endFrame = tm.endFrame;
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
            CollisionTriggerEM em = new CollisionTriggerEM();
            em.startFrame = tm.startFrame;
            em.endFrame = tm.endFrame;
            em.triggerIntervalFrame = tm.triggerIntervalFrame;
            em.triggerMaintainFrame = tm.triggerMaintainFrame;

            var skillEditorTrans = Selection.activeGameObject.transform;
            var colliderRelativePathArray = tm.colliderRelativePathArray;
            var pathCount = colliderRelativePathArray.Length;
            GameObject[] colliderGOArray = new GameObject[pathCount];
            for (int i = 0; i < pathCount; i++) {
                var path = colliderRelativePathArray[i];
                var go = skillEditorTrans.Find(path)?.gameObject;
                colliderGOArray[i] = go;
            }
            em.colliderGOArray = colliderGOArray;

            return em;
        }


        #endregion

        #region [HitPower]

        public static HitPowerEM[] GetEMArray_HitPower(HitPowerTM[] tmArray) {
            if (tmArray == null) return null;
            HitPowerEM[] emArray = new HitPowerEM[tmArray.Length];
            for (int i = 0; i < tmArray.Length; i++) {
                emArray[i] = GetEM_HitPower(tmArray[i]);
            }
            return emArray;
        }

        public static HitPowerEM GetEM_HitPower(this HitPowerTM tm) {
            HitPowerEM em = new HitPowerEM();

            // 生命周期
            em.startFrame = tm.startFrame;
            em.endFrame = tm.endFrame;

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

    }

}
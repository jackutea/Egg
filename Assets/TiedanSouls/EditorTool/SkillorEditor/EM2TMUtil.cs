using TiedanSouls.Template;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillorEditor {

    public static class EM2TMUtil {

        #region [Skill]

        public static SkillTM GetTM_Skill(SkillEditorGo editorGo) {
            SkillTM tm = new SkillTM();
            tm.typeID = editorGo.typeID;
            tm.skillName = editorGo.skillName;
            tm.skillType = editorGo.skillType;
            tm.originalSkillTypeID = editorGo.originalSkillTypeID;
            tm.weaponAnimName = editorGo.weaponAnim.name;
            tm.hitPowerArray = GetTM_HitPowerArray(editorGo.hitPowerEMArray);
            tm.collisionTriggerTMArray = GetTMArray_CollisionTrigger(editorGo.colliderTriggerEMArray);
            return tm;
        }

        #endregion

        #region [HitPower]

        public static HitPowerTM[] GetTM_HitPowerArray(HitPowerEM[] ems) {
            HitPowerTM[] tms = new HitPowerTM[ems.Length];
            for (int i = 0; i < ems.Length; i++) {
                tms[i] = GetTM_HitPower(ems[i]);
            }
            return tms;
        }

        public static HitPowerTM GetTM_HitPower(this HitPowerEM em) {
            var logicIntervalTime = GameCollection.LOGIC_INTERVAL_TIME;

            HitPowerTM tm = new HitPowerTM();
            tm.startFrame = em.startFrame;
            tm.endFrame = em.endFrame;

            var baseDamage = em.baseDamage;
            var totalFrame = em.endFrame - em.startFrame + 1;
            int[] damageArray = new int[totalFrame];
            var damageCurve = em.damageCurve;
            for (int i = 0; i < totalFrame; i++) {
                damageArray[i] = Mathf.RoundToInt(damageCurve.Evaluate(logicIntervalTime * i) * baseDamage);
            }
            tm.damageArray = damageArray;

            var baseHitStunFrame = em.baseHitStunFrame;
            int[] hitStunFrameArray = new int[totalFrame];
            var hitStunFrameCurve = em.hitStunFrameCurve;
            for (int i = 0; i < totalFrame; i++) {
                var hitStunFrame = Mathf.RoundToInt(hitStunFrameCurve.Evaluate(logicIntervalTime * i) * baseHitStunFrame);
                hitStunFrameArray[i] = hitStunFrame;
            }
            tm.hitStunFrameArray = hitStunFrameArray;

            // 击退 根据曲线计算每一帧的速度
            var knockBackDistance_cm = em.knockBackDistance_cm;
            var knockBackFrame = em.knockBackFrame;
            var knockBackDisCurve = em.knockBackDisCurve;
            int[] knockBackVelocityArray = new int[knockBackFrame];
            for (int i = 0; i < knockBackFrame; i++) {
                var time1 = logicIntervalTime * i;
                var time2 = time1 + 0.001f;
                var dis1 = knockBackDisCurve.Evaluate(time1) * knockBackDistance_cm;
                var dis2 = knockBackDisCurve.Evaluate(time2) * knockBackDistance_cm;
                var disDiff = dis2 - dis1;
                var speed = disDiff * 1000;
                knockBackVelocityArray[i] = Mathf.RoundToInt(speed);
            }
            tm.knockBackVelocityArray_cm = knockBackVelocityArray;

            // 击飞 根据曲线计算每一帧的速度
            var knockUpHeight_cm = em.knockUpHeight_cm;
            var knockUpFrame = em.knockUpFrame;
            var knockUpDisCurve = em.knockUpDisCurve;
            int[] knockUpVelocityArray = new int[knockUpFrame];
            for (int i = 0; i < knockUpFrame; i++) {
                var time1 = logicIntervalTime * i;
                var time2 = time1 + 0.001f;
                var dis1 = knockUpDisCurve.Evaluate(time1) * knockUpHeight_cm;
                var dis2 = knockUpDisCurve.Evaluate(time2) * knockUpHeight_cm;
                var disDiff = dis2 - dis1;
                var speed = disDiff * 1000;
                knockUpVelocityArray[i] = Mathf.RoundToInt(speed);
            }
            tm.knockUpVelocityArray_cm = knockUpVelocityArray;

            return tm;
        }

        #endregion

        #region [ColliderTrigger]

        public static CollisionTriggerTM[] GetTMArray_CollisionTrigger(CollisionTriggerEM[] emArray) {
            var len = emArray.Length;
            var tmArray = new CollisionTriggerTM[len];
            for (int i = 0; i < len; i++) {
                tmArray[i] = GetTM_CollisionTrigger(emArray[i]);
            }
            return tmArray;
        }

        public static CollisionTriggerTM GetTM_CollisionTrigger(CollisionTriggerEM em) {
            CollisionTriggerTM tm = new CollisionTriggerTM();
            tm.startFrame = em.startFrame;
            tm.endFrame = em.endFrame;
            tm.triggerIntervalFrame = em.triggerIntervalFrame;
            tm.triggerMaintainFrame = em.triggerMaintainFrame;

            tm.colliderTMArray = GetTMArray_Collider(em.colliderGOArray);

            return tm;
        }

        #endregion

        #region [Collider]

        public static ColliderTM[] GetTMArray_Collider(GameObject[] colliderGOArray) {
            var len = colliderGOArray.Length;
            var colliderTMArray = new ColliderTM[len];
            for (int i = 0; i < len; i++) {
                colliderTMArray[i] = GetTM_Collider(colliderGOArray[i]);
            }
            return colliderTMArray;
        }

        public static ColliderTM GetTM_Collider(GameObject colliderGO) {
            ColliderType colliderType = ColliderType.Cube;
            Vector2 localPos = Vector3.zero;
            float angleZ = 0;
            Vector2 size = Vector3.one;

            if (colliderGO.TryGetComponent<Collider2D>(out var collider2D)) {
                colliderType = ColliderType.Cube;
                localPos = colliderGO.transform.localPosition;
                angleZ = colliderGO.transform.localEulerAngles.z;
                size = collider2D.bounds.size;
            } else {
                Debug.LogError("未知的碰撞体类型");
            }

            ColliderTM tm = new ColliderTM();
            tm.colliderType = colliderType;
            tm.localPos = localPos;
            tm.size = size;
            tm.angleZ = angleZ;

            return tm;
        }

        #endregion

    }

}
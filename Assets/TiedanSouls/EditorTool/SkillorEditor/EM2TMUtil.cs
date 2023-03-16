using System.Linq;
using TiedanSouls.Template;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    public static class EM2TMUtil {

        #region [Skill]

        public static SkillTM GetTM_Skill(SkillEditorGO editorGo) {
            SkillTM tm = new SkillTM();

            tm.typeID = editorGo.typeID;
            tm.skillName = editorGo.skillName;
            tm.skillType = editorGo.skillType;

            tm.startFrame = editorGo.startFrame;
            tm.endFrame = editorGo.endFrame;

            tm.originSkillTypeID = editorGo.originSkillTypeID;

            tm.comboSkillCancelTMArray = GetTM_SkillCancel(editorGo.comboSkillCancelEMArray);
            tm.cancelSkillCancelTMArray = GetTM_SkillCancel(editorGo.cancelSkillCancelEMArray);
            tm.weaponAnimName = editorGo.weaponAnimClip == null ? string.Empty : editorGo.weaponAnimClip.name;
            tm.weaponAnimClip_GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(editorGo.weaponAnimClip));

            tm.hitPowerTMArray = GetTM_HitPowerArray(editorGo.hitPowerEMArray);
            tm.collisionTriggerTMArray = GetTMArray_CollisionTrigger(editorGo.colliderTriggerEMArray);

            return tm;
        }

        static SkillCancelTM[] GetTM_SkillCancel(SkillCancelEM[] ems) {
            SkillCancelTM[] tms = new SkillCancelTM[ems.Length];
            for (int i = 0; i < ems.Length; i++) {
                tms[i] = GetTM_SkillCancel(ems[i]);
            }
            return tms;
        }

        static SkillCancelTM GetTM_SkillCancel(SkillCancelEM em) {
            SkillCancelTM tm = new SkillCancelTM();
            tm.skillTypeID = em.skillTypeID;
            tm.startFrame = em.startFrame;
            tm.endFrame = em.endFrame;
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

            // 伤害 根据曲线计算每一帧的伤害
            var baseDamage = em.damageBase;
            var totalFrame = em.endFrame - em.startFrame + 1;
            int[] damageArray = new int[totalFrame];
            var damageCurve = em.damageCurve;
            for (int i = 0; i < totalFrame; i++) {
                damageArray[i] = Mathf.RoundToInt(damageCurve.Evaluate(logicIntervalTime * i) * baseDamage);
            }
            tm.damageArray = damageArray;
            tm.damageBase = baseDamage;

            // 打击顿帧 根据曲线计算每一帧的顿帧数
            var baseHitStunFrame = em.hitStunFrameBase;
            int[] hitStunFrameArray = new int[totalFrame];
            var hitStunFrameCurve = em.hitStunFrameCurve;
            for (int i = 0; i < totalFrame; i++) {
                var hitStunFrame = Mathf.RoundToInt(hitStunFrameCurve.Evaluate(logicIntervalTime * i) * baseHitStunFrame);
                hitStunFrameArray[i] = hitStunFrame;
            }
            tm.hitStunFrameArray = hitStunFrameArray;
            tm.hitStunFrameBase = baseHitStunFrame;

            // 击退 根据曲线计算每一帧的速度
            var knockBackDistance_cm = em.knockBackDistance_cm;
            var knockBackCostFrame = em.knockBackCostFrame;
            var knockBackDisCurve = em.knockBackDisCurve;
            int[] knockBackSpeedArray = new int[knockBackCostFrame];
            for (int i = 0; i < knockBackCostFrame; i++) {
                var time1 = logicIntervalTime * i;
                var time2 = time1 + 0.001f;
                var dis1 = knockBackDisCurve.Evaluate(time1) * knockBackDistance_cm;
                var dis2 = knockBackDisCurve.Evaluate(time2) * knockBackDistance_cm;
                var disDiff = dis2 - dis1;
                var speed = disDiff * 1000;
                knockBackSpeedArray[i] = Mathf.RoundToInt(speed);
            }
            tm.knockBackSpeedArray_cm = knockBackSpeedArray;
            tm.knockBackCostFrame = knockBackCostFrame;
            tm.knockBackDistance_cm = knockBackDistance_cm;

            // 击飞 根据曲线计算每一帧的速度
            var knockUpHeight_cm = em.knockUpHeight_cm;
            var knockUpCostFrame = em.knockUpCostFrame;
            var knockUpDisCurve = em.knockUpDisCurve;
            int[] knockUpSpeedArray = new int[knockUpCostFrame];
            for (int i = 0; i < knockUpCostFrame; i++) {
                var time1 = logicIntervalTime * i;
                var time2 = time1 + 0.001f;
                var dis1 = knockUpDisCurve.Evaluate(time1) * knockUpHeight_cm;
                var dis2 = knockUpDisCurve.Evaluate(time2) * knockUpHeight_cm;
                var disDiff = dis2 - dis1;
                var speed = disDiff * 1000;
                knockUpSpeedArray[i] = Mathf.RoundToInt(speed);
            }
            tm.knockUpSpeedArray_cm = knockUpSpeedArray;
            tm.knockUpCostFrame = knockUpCostFrame;
            tm.knockUpHeight_cm = knockUpHeight_cm;

            // 曲线 Keyframe 保存
            tm.damageCurve_KeyframeTMArray = GetTMArray_Keyframe(damageCurve.keys);
            tm.hitStunFrameCurve_KeyframeTMArray = GetTMArray_Keyframe(hitStunFrameCurve.keys);
            tm.knockBackDisCurve_KeyframeTMArray = GetTMArray_Keyframe(knockBackDisCurve.keys);
            tm.knockUpDisCurve_KeyframeTMArray = GetTMArray_Keyframe(knockUpDisCurve.keys);

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
            tm.colliderRelativePathArray = GetRelativePathArray(em.colliderGOArray);

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
            if (colliderGO == null) {
                Debug.LogWarning("碰撞器为空");
                return null;
            }

            ColliderType colliderType = ColliderType.Cube;
            Vector3 localPos = Vector3.zero;
            float angleZ = 0;
            Vector3 size = Vector3.one;

            if (colliderGO.TryGetComponent<Collider2D>(out var collider2D)) {
                colliderType = ColliderType.Cube;
                localPos = colliderGO.transform.position;
                angleZ = colliderGO.transform.localEulerAngles.z;
                size = collider2D.transform.lossyScale;
            } else {
                Debug.LogError($"未知的碰撞器类型 {colliderGO.name}");
            }

            ColliderTM tm = new ColliderTM();
            tm.colliderType = colliderType;
            tm.localPos = localPos;
            tm.size = size;
            tm.localAngleZ = angleZ;
            Debug.Log($"碰撞器 {colliderGO.name} 本地坐标 {localPos} 本地角度 {angleZ} 大小 {size}");

            return tm;
        }

        #endregion

        #region [KeyFrame]

        public static KeyframeTM[] GetTMArray_Keyframe(Keyframe[] keyframeArray) {
            var len = keyframeArray.Length;
            var keyFrameArray = new KeyframeTM[len];
            for (int i = 0; i < len; i++) {
                keyFrameArray[i] = new KeyframeTM(keyframeArray[i]);
            }
            return keyFrameArray;
        }

        #endregion

        #region [Misc]

        static string[] GetRelativePathArray(GameObject[] goArray) {
            var len = goArray.Length;
            var pathArray = new string[len];
            for (int i = 0; i < len; i++) {
                var go = goArray[i];
                if (go == null) continue;
                pathArray[i] = GetRelativePath(go.transform);
            }
            return pathArray;
        }

        static string GetRelativePath(Transform begin) {
            if (begin == null) return "";

            string path = begin.name;
            Transform parent = begin.parent;
            if (parent == null) return path;

            while (parent != null && !parent.TryGetComponent<SkillEditorGO>(out _)) {
                path = $"{parent.name}/{path}";
                parent = parent.parent;
            }
            return path;
        }

        #endregion

    }

}
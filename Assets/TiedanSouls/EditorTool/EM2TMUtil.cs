using System.Linq;
using TiedanSouls.EditorTool.EffectorEditor;
using TiedanSouls.EditorTool.SkillEditor;
using TiedanSouls.Generic;
using TiedanSouls.Template;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    public static class EM2TMUtil {

        #region [Projectile]

        public static ProjectileTM GetTM_Projectile(ProjectileEditorGO editorGo) {
            ProjectileTM tm;
            tm.typeID = editorGo.typeID;
            tm.projectileName = editorGo.projectileName;
            tm.rootElement = GetTM_ProjectileElement(editorGo.rootElement);
            tm.leafElementTMArray = GetTMArray_ProjectileElement(editorGo.leafElementEMArray);
            return tm;
        }

        public static ProjectileElementTM[] GetTMArray_ProjectileElement(ProjectileElementEM[] emArray) {
            var tmArray = new ProjectileElementTM[emArray.Length];
            for (int i = 0; i < emArray.Length; i++) {
                tmArray[i] = GetTM_ProjectileElement(emArray[i]);
            }
            return tmArray;
        }

        public static ProjectileElementTM GetTM_ProjectileElement(ProjectileElementEM em) {
            ProjectileElementTM tm;
            tm.startFrame = em.startFrame;
            tm.endFrame = em.endFrame;
            tm.collisionTriggerTM = GetTM_CollisionTrigger(em.collisionTriggerEM);
            tm.hitEffectorTM = GetTM_Effector(em.hitEffectorEM);
            tm.deathEffectorTM = GetTM_Effector(em.deathEffectorEM);
            tm.extraHitTimes = em.extraHitTimes;
            tm.vfxPrefabName = em.vfxPrefab == null ? string.Empty : em.vfxPrefab.name;
            tm.vfxPrefab_GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(em.vfxPrefab));
            return tm;
        }

        #endregion

        #region [Effector]

        public static EffectorTM GetTM_Effector(EffectorEM em) {
            EffectorTM tm;
            tm.typeID = em.typeID;
            tm.effectorName = em.effectorName;
            tm.entitySummonTMArray = GetTMArray_EntitySummon(em.entitySummonEMArray);
            tm.entityDestroyTMArray = GetTMArray_EntityDestroy(em.entityDestroyEMArray);
            return tm;
        }

        #endregion

        #region [Skill]

        public static SkillTM GetTM_Skill(SkillEditorGO editorGo) {
            SkillTM tm;

            tm.typeID = editorGo.typeID;
            tm.skillName = editorGo.skillName;
            tm.skillType = editorGo.skillType;
            tm.maintainFrame = editorGo.maintainFrame;

            tm.originSkillTypeID = editorGo.originSkillTypeID;
            tm.comboSkillCancelTMArray = GetTM_SkillCancel(editorGo.comboSkillCancelEMArray);
            tm.cancelSkillCancelTMArray = GetTM_SkillCancel(editorGo.cancelSkillCancelEMArray);

            tm.skillEffectorTMArray = GetTMArray_SkillEffector(editorGo.skillEffectorEMArray);

            tm.weaponAnimName = editorGo.weaponAnimClip == null ? string.Empty : editorGo.weaponAnimClip.name;
            tm.weaponAnimClip_GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(editorGo.weaponAnimClip));


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
            SkillCancelTM tm;
            tm.skillTypeID = em.skillTypeID;
            tm.startFrame = em.startFrame;
            tm.endFrame = em.endFrame;
            return tm;
        }

        static SkillEffectorTM[] GetTMArray_SkillEffector(SkillEffectorEM[] ems) {
            SkillEffectorTM[] tms = new SkillEffectorTM[ems.Length];
            for (int i = 0; i < ems.Length; i++) {
                tms[i] = GetTM_SkillEffector(ems[i]);
            }
            return tms;
        }

        static SkillEffectorTM GetTM_SkillEffector(SkillEffectorEM em) {
            SkillEffectorTM tm;
            tm.triggerFrame = em.triggerFrame;
            tm.effectorTypeID = em.effectorTypeID;
            tm.offsetPos = em.offsetPos;
            return tm;
        }

        #endregion

        #region [HitPower]

        public static HitPowerTM GetTM_HitPower(HitPowerEM em, int startFrame, int endFrame) {
            var logicIntervalTime = GameCollection.LOGIC_INTERVAL_TIME;

            HitPowerTM tm;

            // 伤害 根据曲线计算每一帧的伤害
            var baseDamage = em.damageBase;
            var totalFrame = endFrame - startFrame + 1;
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
            var startFrame = em.startFrame;
            var endFrame = em.endFrame;

            CollisionTriggerTM tm;

            tm.isEnabled = em.isEnabled;
            tm.startFrame = startFrame;
            tm.endFrame = endFrame;

            tm.delayFrame = em.delayFrame;
            tm.intervalFrame = em.intervalFrame;
            tm.maintainFrame = em.maintainFrame;

            tm.colliderTMArray = GetTMArray_Collider(em.colliderGOArray);
            tm.hitPowerTM = GetTM_HitPower(em.hitPowerEM, startFrame, endFrame);
            tm.hitTargetType = em.hitTargetType;

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
                return default;
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

            ColliderTM tm;
            tm.colliderType = colliderType;
            tm.localPos = localPos;
            tm.size = size;
            tm.localAngleZ = angleZ;

            return tm;
        }

        #endregion

        #region [Entity Summon]

        public static EntitySummonTM[] GetTMArray_EntitySummon(EntitySummonEM[] emArray) {
            var len = emArray.Length;
            var tmArray = new EntitySummonTM[len];
            for (int i = 0; i < len; i++) {
                tmArray[i] = GetTM_EntitySummon(emArray[i]);
            }
            return tmArray;
        }

        public static EntitySummonTM GetTM_EntitySummon(EntitySummonEM em) {
            EntitySummonTM tm;
            tm.entityType = em.entityType;
            tm.typeID = em.typeID;
            tm.controlType = em.controlType;
            return tm;
        }

        #endregion

        #region [Entity Destroy]

        public static EntityDestroyTM[] GetTMArray_EntityDestroy(EntityDestroyEM[] emArray) {
            var len = emArray.Length;
            var tmArray = new EntityDestroyTM[len];
            for (int i = 0; i < len; i++) {
                tmArray[i] = GetTM_EntityDestroy(emArray[i]);
            }
            return tmArray;
        }

        public static EntityDestroyTM GetTM_EntityDestroy(EntityDestroyEM em) {
            EntityDestroyTM tm;
            tm.entityType = em.entityType;
            tm.targetType = em.targetType;
            tm.isEnabled_attributeSelector = em.isEnabled_attributeSelector;
            tm.attributeSelectorTM = GetTM_AttributeSelector(em.attributeSelectorEM);
            return tm;
        }

        #endregion

        #region [Selector]

        public static AttributeSelectorTM[] GetTMArray_AttributeSelector(AttributeSelectorEM[] emArray) {
            var len = emArray.Length;
            var tmArray = new AttributeSelectorTM[len];
            for (int i = 0; i < len; i++) {
                tmArray[i] = GetTM_AttributeSelector(emArray[i]);
            }
            return tmArray;
        }

        public static AttributeSelectorTM GetTM_AttributeSelector(AttributeSelectorEM em) {
            AttributeSelectorTM tm;
            tm.hp = em.hp;
            tm.hp_ComparisonType = em.hp_ComparisonType;
            tm.hpMax = em.hpMax;
            tm.hpMax_ComparisonType = em.hpMax_ComparisonType;
            tm.ep = em.ep;
            tm.ep_ComparisonType = em.ep_ComparisonType;
            tm.epMax = em.epMax;
            tm.epMax_ComparisonType = em.epMax_ComparisonType;
            tm.gp = em.gp;
            tm.gp_ComparisonType = em.gp_ComparisonType;
            tm.gpMax = em.gpMax;
            tm.gpMax_ComparisonType = em.gpMax_ComparisonType;
            tm.moveSpeed = em.moveSpeed;
            tm.moveSpeed_ComparisonType = em.moveSpeed_ComparisonType;
            tm.jumpSpeed = em.jumpSpeed;
            tm.jumpSpeed_ComparisonType = em.jumpSpeed_ComparisonType;
            tm.fallingAcceleration = em.fallingAcceleration;
            tm.fallingAcceleration_ComparisonType = em.fallingAcceleration_ComparisonType;
            tm.fallingSpeedMax = em.fallingSpeedMax;
            tm.fallingSpeedMax_ComparisonType = em.fallingSpeedMax_ComparisonType;
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
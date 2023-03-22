using System.Linq;
using GameArki.AddressableHelper;
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
            tm.rootElementTM = GetTM_ProjectileElement(editorGo.rootElement);
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
            tm.collisionTriggerTM = GetTM_CollisionTrigger<ProjectileEditorGO>(em.collisionTriggerEM);
            tm.hitEffectorTM = GetTM_Effector(em.hitEffectorEM);
            tm.deathEffectorTM = GetTM_Effector(em.deathEffectorEM);
            tm.extraHitTimes = em.extraHitTimes;
            tm.vfxPrefabName = em.vfxPrefab == null ? string.Empty : em.vfxPrefab.name;
            tm.vfxPrefab_GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(em.vfxPrefab));

            var labelName = AssetsLabelCollection.VFX;
            AddressableHelper.SetAddressable(em.vfxPrefab, labelName, labelName);

            var moveDistance_cm = em.moveDistance_cm;
            var moveTotalFrame = em.moveTotalFrame;
            var moveCurve = em.moveCurve;
            var moveKeyFrameArray = em.moveCurve.keys;
            tm.keyframeTMArray = GetTMArray_Keyframe(moveKeyFrameArray);
            tm.moveDistance = em.moveDistance_cm;
            tm.moveTotalFrame = em.moveTotalFrame;
            tm.moveSpeedArray_cm = GetSpeedArray_AnimationCurve(moveDistance_cm, moveTotalFrame, moveCurve);
            tm.directionArray = GetDirectionArray_AnimationCurve(moveTotalFrame);
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


            tm.collisionTriggerTMArray = GetTMArray_CollisionTrigger<SkillEditorGO>(editorGo.colliderTriggerEMArray);

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
            HitPowerTM tm;

            // 伤害 根据曲线计算每一帧的伤害
            var damageBase = em.damageBase;
            var totalFrame_damage = endFrame - startFrame + 1;
            var damageCurve = em.damageCurve;
            var damageArray = GetValueArray_AnimationCurve(damageBase, totalFrame_damage, damageCurve);
            tm.damageArray = damageArray;
            tm.damageBase = damageBase;

            // 打击顿帧 根据曲线计算每一帧的顿帧数
            var hitStunFrameBase = em.hitStunFrameBase;
            var totalFrame_hitStun = endFrame - startFrame + 1;
            var hitStunFrameCurve = em.hitStunFrameCurve;
            var hitStunFrameArray = GetValueArray_AnimationCurve(hitStunFrameBase, totalFrame_hitStun, hitStunFrameCurve);
            tm.hitStunFrameArray = hitStunFrameArray;
            tm.hitStunFrameBase = hitStunFrameBase;

            // 击退 根据曲线计算每一帧的速度
            var knockBackDistance_cm = em.knockBackDistance_cm;
            var knockBackCostFrame = em.knockBackCostFrame;
            var knockBackDisCurve = em.knockBackDisCurve;
            var knockBackSpeedArray_cm = GetSpeedArray_AnimationCurve(knockBackDistance_cm, knockBackCostFrame, knockBackDisCurve);
            tm.knockBackCostFrame = knockBackCostFrame;
            tm.knockBackDistance_cm = knockBackDistance_cm;
            tm.knockBackSpeedArray_cm = knockBackSpeedArray_cm;

            // 击飞 根据曲线计算每一帧的速度
            var knockUpHeight_cm = em.knockUpHeight_cm;
            var knockUpCostFrame = em.knockUpCostFrame;
            var knockUpDisCurve = em.knockUpDisCurve;
            var knockUpSpeedArray_cm = GetSpeedArray_AnimationCurve(knockUpHeight_cm, knockUpCostFrame, knockUpDisCurve);
            tm.knockUpCostFrame = knockUpCostFrame;
            tm.knockUpHeight_cm = knockUpHeight_cm;
            tm.knockUpSpeedArray_cm = knockUpSpeedArray_cm;

            // 曲线 KeyframeTM 保存
            tm.damageCurve_KeyframeTMArray = GetTMArray_Keyframe(damageCurve.keys);
            tm.hitStunFrameCurve_KeyframeTMArray = GetTMArray_Keyframe(hitStunFrameCurve.keys);
            tm.knockBackDisCurve_KeyframeTMArray = GetTMArray_Keyframe(knockBackDisCurve.keys);
            tm.knockUpDisCurve_KeyframeTMArray = GetTMArray_Keyframe(knockUpDisCurve.keys);

            return tm;
        }

        #endregion

        #region [ColliderTrigger]

        public static CollisionTriggerTM[] GetTMArray_CollisionTrigger<T>(CollisionTriggerEM[] emArray) {
            var len = emArray.Length;
            var tmArray = new CollisionTriggerTM[len];
            for (int i = 0; i < len; i++) {
                tmArray[i] = GetTM_CollisionTrigger<T>(emArray[i]);
            }
            return tmArray;
        }

        public static CollisionTriggerTM GetTM_CollisionTrigger<T>(CollisionTriggerEM em) {
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

            tm.colliderRelativePathArray = GetRelativePathArray_SkillEditor<T>(em.colliderGOArray);

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

        #region [Animation Curve]

        public static int[] GetValueArray_AnimationCurve(int baseV, int totalFrame, AnimationCurve curve) {
            int[] damageArray = new int[totalFrame];
            for (int i = 0; i < totalFrame; i++) {
                damageArray[i] = Mathf.RoundToInt(curve.Evaluate(totalFrame * i) * baseV);
            }
            return damageArray;
        }

        public static int[] GetSpeedArray_AnimationCurve(int totalDis, int totalFrame, AnimationCurve curve) {
            float logicIntervalTime = GameCollection.LOGIC_INTERVAL_TIME;
            float totalTime = totalFrame * logicIntervalTime;
            int[] speedArray = new int[totalFrame];
            for (int i = 0; i < totalFrame; i++) {
                float norTime1 = logicIntervalTime * i / totalTime;
                float norTime2 = logicIntervalTime * (i + 1) / totalTime;
                float dis1 = curve.Evaluate(norTime1) * totalDis;
                float dis2 = curve.Evaluate(norTime2) * totalDis;
                float disDiff = dis2 - dis1;
                float speed = disDiff / logicIntervalTime;
                speedArray[i] = Mathf.RoundToInt(speed);
            }
            return speedArray;
        }

        public static Vector3[] GetDirectionArray_AnimationCurve(int totalFrame) {
            Vector3[] directionArray = new Vector3[totalFrame];
            for (int i = 0; i < totalFrame; i++) {
                directionArray[i] = Vector3.right;
            }
            return directionArray;
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

        static string[] GetRelativePathArray_SkillEditor<T>(GameObject[] goArray) {
            var len = goArray.Length;
            var pathArray = new string[len];
            for (int i = 0; i < len; i++) {
                var go = goArray[i];
                if (go == null) continue;
                pathArray[i] = GetRelativePath_SkillEditor<T>(go.transform);
            }
            return pathArray;
        }

        static string GetRelativePath_SkillEditor<T>(Transform begin) {
            if (begin == null) return "";

            string path = begin.name;
            Transform parent = begin.parent;
            if (parent == null) return path;

            while (parent != null && !parent.TryGetComponent<T>(out _)) {
                path = $"{parent.name}/{path}";
                parent = parent.parent;
            }
            return path;
        }

        #endregion

    }

}
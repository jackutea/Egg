using GameArki.AddressableHelper;
using TiedanSouls.Generic;
using TiedanSouls.Template;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    public static class EM2TMUtil {

        #region [Buff]

        public static BuffTM GetBuffTM(BuffEditorGO editorGo) {
            BuffTM tm;

            tm.typeID = editorGo.typeID;
            tm.buffName = editorGo.buffName;
            tm.description = editorGo.description;
            tm.iconName = editorGo.icon.name;
            tm.iconGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(editorGo.icon));

            tm.delayFrame = editorGo.delayFrame;
            tm.intervalFrame = editorGo.intervalFrame;
            tm.durationFrame = editorGo.durationFrame;

            tm.attributeEffectTM = GetAttributeEffectTM(editorGo.attributeEffectEM);
            tm.effectorTypeID = editorGo.effectorTypeID;

            return tm;
        }

        #endregion

        #region [AttributeEffect]

        public static AttributeEffectTM[] GetAttributeEffectTMArray(AttributeEffectEM[] emArray) {
            AttributeEffectTM[] tmArray = new AttributeEffectTM[emArray.Length];
            for (int i = 0; i < emArray.Length; i++) {
                tmArray[i] = GetAttributeEffectTM(emArray[i]);
            }
            return tmArray;
        }

        public static AttributeEffectTM GetAttributeEffectTM(AttributeEffectEM em) {
            AttributeEffectTM tm;

            tm.hpNCT = em.hpNCT;
            tm.hpEV_Expanded = GetInt_Expand100(em.hpEV);
            tm.needRevokeHP = em.needRevoke_HPEV;
            tm.hpEffectTimes = em.hpEffectTimes;

            tm.hpMaxNCT = em.hpMaxNCT;
            tm.hpMaxEV_Expanded = GetInt_Expand100(em.hpMaxEV);
            tm.needRevokeHPMax = em.needRevoke_HPMaxEV;
            tm.hpMaxEffectTimes = em.hpMaxEffectTimes;

            tm.moveSpeedNCT = em.moveSpeedNCT;
            tm.moveSpeedEV_Expanded = GetInt_Expand100(em.moveSpeedEV);
            tm.needRevokeMoveSpeed = em.needRevoke_MoveSpeedEV;
            tm.moveSpeedEffectTimes = em.moveSpeedEffectTimes;

            tm.physicalDamageBonusEV_Expanded = GetInt_Expand100(em.physicalDamageBonusEV);
            tm.physicalDamageBonusEffectTimes = em.physicalDamageBonusEffectTimes;
            tm.needRevokePhysicalDamageBonus = em.needRevokePhysicalDamageBonus;

            tm.magicalDamageBonusEV_Expanded = GetInt_Expand100(em.magicalDamageBonusEV);
            tm.magicalDamageBonusEffectTimes = em.magicalDamageBonusEffectTimes;
            tm.needRevokemagicalDamageBonus = em.needRevokemagicalDamageBonus;

            tm.physicalDefenseBonusEV_Expanded = GetInt_Expand100(em.physicalDefenseBonusEV);
            tm.physicalDefenseBonusEffectTimes = em.physicalDefenseBonusEffectTimes;
            tm.needRevokePhysicalDefenseBonus = em.needRevokePhysicalDefenseBonus;

            tm.magicalDefenseBonusEV_Expanded = GetInt_Expand100(em.magicalDefenseBonusEV);
            tm.magicalDefenseBonusEffectTimes = em.magicalDefenseBonusEffectTimes;
            tm.needRevokemagicalDefenseBonus = em.needRevokemagicalDefenseBonus;

            return tm;
        }

        #endregion

        #region [Projectile]

        public static ProjectileTM GetProjectileTM(ProjectileEditorGO editorGo) {
            ProjectileTM tm;

            tm.typeID = editorGo.typeID;
            tm.projectileName = editorGo.projectileName;
            tm.projetileBulletTMArray = GetProjectleBulletTMArray(editorGo.projectileBulletEMArray);

            return tm;
        }


        #endregion

        #region [ProjectileBullet]

        public static ProjectileBulletTM[] GetProjectleBulletTMArray(ProjectileBulletEM[] emArray) {
            var tmArray = new ProjectileBulletTM[emArray.Length];
            for (int i = 0; i < emArray.Length; i++) {
                tmArray[i] = GetProjectleBulletTM(emArray[i]);
            }
            return tmArray;
        }

        public static ProjectileBulletTM GetProjectleBulletTM(ProjectileBulletEM em) {
            ProjectileBulletTM tm;

            tm.startFrame = em.startFrame;
            tm.localPos_cm = GetVector3Int_Expand100(em.localPos);
            tm.localEulerAngles = em.localEulerAngles;
            tm.bulletTypeID = em.bulletTypeID;

            return tm;
        }

        #endregion

        #region [Bullet]

        public static BulletTM GetBulletTM(BulletEditorGO editorGO) {
            BulletTM tm;

            tm.typeID = editorGO.typeID;
            tm.bulletName = editorGO.bulletName;

            tm.maintainFrame = editorGO.maintainFrame;
            tm.extraPenetrateCount = editorGO.extraPenetrateCount;
            tm.deathEffectorTypeID = editorGO.deathEffectorTypeID;
            tm.collisionTriggerTM = GetCollisionTriggerTM<BulletEditorGO>(editorGO.collisionTriggerEM);

            tm.trajectoryType = editorGO.trajectoryType;

            tm.entityTrackTM = GetEntityTrackTM(editorGO.entityTrackingEM);
            tm.moveCurveTM = GetMoveCurveTM(editorGO.moveCurveEM);

            var vfxPrefab = editorGO.vfxPrefab;
            tm.vfxPrefabName = vfxPrefab == null ? string.Empty : vfxPrefab.name;
            tm.vfxPrefab_GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(vfxPrefab));

            var labelName = AssetLabelCollection.VFX;
            AddressableHelper.SetAddressable(vfxPrefab, labelName, labelName);

            return tm;
        }

        #endregion

        #region [Skill]

        public static SkillTM GetSkillTM(SkillEditorGO editorGo) {
            SkillTM tm;

            tm.typeID = editorGo.typeID;
            tm.skillName = editorGo.skillName;
            tm.skillType = editorGo.skillType;
            tm.maintainFrame = editorGo.maintainFrame;

            tm.originSkillTypeID = editorGo.originSkillTypeID;
            tm.comboSkillCancelTMArray = GetSkillCancelTM(editorGo.comboSkillCancelEMArray);
            tm.cancelSkillCancelTMArray = GetSkillCancelTM(editorGo.cancelSkillCancelEMArray);

            tm.skillEffectorTMArray = GetSkillEffectorTMArray(editorGo.skillEffectorEMArray);

            tm.weaponAnimName = editorGo.weaponAnimClip == null ? string.Empty : editorGo.weaponAnimClip.name;
            tm.weaponAnimClip_GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(editorGo.weaponAnimClip));

            tm.collisionTriggerTMArray = GetCollisionTriggerTMArray<SkillEditorGO>(editorGo.colliderTriggerEMArray);

            tm.skillMoveCurveTMArray = GetMoveCurveTMArray(editorGo.skillMoveCurveEMArray);

            return tm;
        }

        public static SkillMoveCurveTM[] GetMoveCurveTMArray(SkillMoveCurveEM[] emArray) {
            SkillMoveCurveTM[] tmArray = new SkillMoveCurveTM[emArray.Length];
            for (int i = 0; i < emArray.Length; i++) {
                tmArray[i] = GetMoveCurveTM(emArray[i]);
            }
            return tmArray;
        }

        public static SkillMoveCurveTM GetMoveCurveTM(SkillMoveCurveEM em) {
            SkillMoveCurveTM tm;
            tm.startFrame = em.startFrame;
            tm.isFaceTo = em.isFaceTo;
            tm.needWaitForMoveEnd = em.needWaitForMoveEnd;
            tm.moveCurveTM = GetMoveCurveTM(em.moveCurveEM);
            return tm;
        }

        public static SkillTM[] GetSkillTMArray(SkillEditorGO[] editorGoArray) {
            SkillTM[] tmArray = new SkillTM[editorGoArray.Length];
            for (int i = 0; i < editorGoArray.Length; i++) {
                tmArray[i] = GetSkillTM(editorGoArray[i]);
            }
            return tmArray;
        }

        public static SkillCancelTM[] GetSkillCancelTM(SkillCancelEM[] ems) {
            SkillCancelTM[] tms = new SkillCancelTM[ems.Length];
            for (int i = 0; i < ems.Length; i++) {
                tms[i] = GetSkillCancelTM(ems[i]);
            }
            return tms;
        }

        public static SkillCancelTM GetSkillCancelTM(SkillCancelEM em) {
            SkillCancelTM tm;
            tm.skillTypeID = em.skillTypeID;
            tm.startFrame = em.startFrame;
            tm.endFrame = em.endFrame;
            return tm;
        }

        public static SkillEffectorTM[] GetSkillEffectorTMArray(SkillEffectorEM[] ems) {
            SkillEffectorTM[] tms = new SkillEffectorTM[ems.Length];
            for (int i = 0; i < ems.Length; i++) {
                tms[i] = GetSkillEffectorTM(ems[i]);
            }
            return tms;
        }

        public static SkillEffectorTM GetSkillEffectorTM(SkillEffectorEM em) {
            SkillEffectorTM tm;
            tm.triggerFrame = em.triggerFrame;
            tm.effectorTypeID = em.effectorTypeID;
            tm.offsetPos = em.offsetPos;
            return tm;
        }

        #endregion

        #region [Effector]

        public static EffectorTM GetEffectorTM(EffectorEM em) {
            EffectorTM tm;
            tm.typeID = em.typeID;
            tm.effectorName = em.effectorName;
            tm.entitySummonTMArray = GetEntitySummonTMArray(em.entitySummonEMArray);
            tm.entityDestroyTMArray = GetEntityDestroyTMArray(em.entityDestroyEMArray);
            return tm;
        }

        #endregion

        #region [PhysicsPower]

        /// <summary>
        /// 击退 根据曲线计算每一帧的速度
        /// </summary>
        public static KnockBackTM GetKnockBackTM(KnockBackEM em) {
            KnockBackTM tm;
            var knockBackDistance_cm = GetInt_Expand100(em.knockBackDistance);
            var knockBackCostFrame = em.knockBackCostFrame;
            var knockBackDisCurve = em.knockBackDisCurve;
            var knockBackSpeedArray_cm = GetSpeedArray_AnimationCurve(knockBackDistance_cm, knockBackCostFrame, knockBackDisCurve);
            tm.knockBackCostFrame = knockBackCostFrame;
            tm.knockBackDistance_cm = knockBackDistance_cm;
            tm.knockBackSpeedArray_cm = knockBackSpeedArray_cm;
            tm.knockBackDisCurve_KeyframeTMArray = GetKeyframeTMArray(knockBackDisCurve);
            return tm;
        }

        /// <summary>
        /// 击飞 根据曲线计算每一帧的速度
        /// </summary>
        public static KnockUpTM GetKnockUpTM(KnockUpEM em) {
            KnockUpTM tm;
            var knockUpHeight_cm = GetInt_Expand100(em.knockUpHeight);
            var knockUpCostFrame = em.knockUpCostFrame;
            var knockUpDisCurve = em.knockUpDisCurve;
            var knockUpSpeedArray_cm = GetSpeedArray_AnimationCurve(knockUpHeight_cm, knockUpCostFrame, knockUpDisCurve);
            tm.knockUpCostFrame = knockUpCostFrame;
            tm.knockUpHeight_cm = knockUpHeight_cm;
            tm.knockUpSpeedArray_cm = knockUpSpeedArray_cm;
            tm.knockUpDisCurve_KeyframeTMArray = GetKeyframeTMArray(knockUpDisCurve);
            return tm;
        }

        #endregion

        #region [Damage]

        public static DamageTM GetDamageTM(DamageEM em, int totalFrame) {
            var damageBase = em.damageBase;
            var damageCurve = em.damageCurve;

            DamageTM tm;
            tm.damageType = em.damageType;
            tm.damageBase = damageBase;
            tm.damageArray = GetValueArray_AnimationCurve(damageBase, totalFrame, damageCurve);
            tm.damageCurve_KeyframeTMArray = GetKeyframeTMArray(damageCurve);

            return tm;
        }

        #endregion

        #region [ColliderTrigger]

        public static EntityColliderTriggerTM[] GetCollisionTriggerTMArray<T>(EntityColliderTriggerEM[] emArray) {
            var len = emArray.Length;
            var tmArray = new EntityColliderTriggerTM[len];
            for (int i = 0; i < len; i++) {
                tmArray[i] = GetCollisionTriggerTM<T>(emArray[i]);
            }
            return tmArray;
        }

        public static EntityColliderTriggerTM GetCollisionTriggerTM<T>(EntityColliderTriggerEM em) {
            var frameRange = em.frameRange;
            var totalFrame = frameRange.y - frameRange.x + 1;

            EntityColliderTriggerTM tm;
            tm.isEnabled = em.isEnabled;
            tm.frameRange = frameRange;

            tm.triggerMode = em.triggerMode;
            tm.triggerFixedIntervalTM = GetTriggerFixedIntervalTM(em.triggerFixedIntervalEM);
            tm.triggerCustomTM = GetTriggerCustomTM(em.triggerCustomEM);

            tm.colliderTMArray = GetColliderTMArray(em.colliderGOArray);

            tm.targetEntityType = em.targetEntityType;
            tm.hitTargetGroupType = em.hitTargetGroupType;

            tm.damageTM = GetDamageTM(em.damageEM, totalFrame);
            tm.knockBackPowerTM = GetKnockBackTM(em.knockBackEM);
            tm.knockUpPowerTM = GetKnockUpTM(em.knockUpEM);
            tm.hitEffectorTypeID = em.hitEffectorTypeID;

            tm.colliderRelativePathArray = GetRelativePathArray<T>(em.colliderGOArray);

            return tm;
        }

        public static TriggerFixedIntervalTM GetTriggerFixedIntervalTM(TriggerFixedIntervalEM em) {
            TriggerFixedIntervalTM tm;
            tm.delayFrame = em.delayFrame;
            tm.intervalFrame = em.intervalFrame;
            tm.maintainFrame = em.maintainFrame;
            return tm;
        }

        public static TriggerCustomTM GetTriggerCustomTM(TriggerCustomEM em) {
            TriggerCustomTM tm;
            tm.frameRangeArray = em.frameRangeArray?.Clone() as Vector2Int[];
            return tm;
        }

        #endregion

        #region [Collider]

        public static ColliderTM[] GetColliderTMArray(GameObject[] colliderGOArray) {
            var len = colliderGOArray.Length;
            var colliderTMArray = new ColliderTM[len];
            for (int i = 0; i < len; i++) {
                colliderTMArray[i] = GetColliderTM(colliderGOArray[i]);
            }
            return colliderTMArray;
        }

        public static ColliderTM GetColliderTM(GameObject colliderGO) {
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
            tm.localPosition = localPos;
            tm.localScale = size;
            tm.localAngleZ = angleZ;

            return tm;
        }

        #endregion

        #region [Entity Summon]

        public static EntitySummonTM[] GetEntitySummonTMArray(EntitySummonEM[] emArray) {
            var len = emArray.Length;
            var tmArray = new EntitySummonTM[len];
            for (int i = 0; i < len; i++) {
                tmArray[i] = GetEntitySummonTM(emArray[i]);
            }
            return tmArray;
        }

        public static EntitySummonTM GetEntitySummonTM(EntitySummonEM em) {
            EntitySummonTM tm;
            tm.entityType = em.entityType;
            tm.typeID = em.typeID;
            tm.controlType = em.controlType;
            return tm;
        }

        #endregion

        #region [Entity Destroy]

        public static EntityDestroyTM[] GetEntityDestroyTMArray(EntityDestroyEM[] emArray) {
            var len = emArray.Length;
            var tmArray = new EntityDestroyTM[len];
            for (int i = 0; i < len; i++) {
                tmArray[i] = GetEntityDestroyTM(emArray[i]);
            }
            return tmArray;
        }

        public static EntityDestroyTM GetEntityDestroyTM(EntityDestroyEM em) {
            EntityDestroyTM tm;
            tm.entityType = em.entityType;
            tm.hitTargetGroupType = em.hitTargetGroupType;
            tm.attributeSelector_IsEnabled = em.attributeSelector_IsEnabled;
            tm.attributeSelectorTM = GetAttributeSelectorTM(em.attributeSelectorEM);
            return tm;
        }

        #endregion

        #region [EntityTrack]

        public static EntityTrackTM GetEntityTrackTM(EntityTrackEM em) {
            EntityTrackTM tm;
            tm.trackSpeed_cm = GetInt_Expand100(em.trackSpeed);
            tm.trackTargetGroupType = em.trackTargetGroupType;
            tm.entityTrackSelectorTM = GetEntityTrackSelectorTM(em.entityTrackSelectorEM);
            return tm;
        }

        public static EntityTrackSelectorTM GetEntityTrackSelectorTM(EntityTrackSelectorEM em) {
            EntityTrackSelectorTM tm;
            tm.entityType = em.entityType;
            tm.isAttributeSelectorEnabled = em.isAttributeSelectorEnabled;
            tm.attributeSelectorTM = GetAttributeSelectorTM(em.attributeSelectorEM);
            return tm;
        }

        #endregion

        #region [Selector]

        public static AttributeSelectorTM[] GetAttributeSelectorTMArray(AttributeSelectorEM[] emArray) {
            var len = emArray.Length;
            var tmArray = new AttributeSelectorTM[len];
            for (int i = 0; i < len; i++) {
                tmArray[i] = GetAttributeSelectorTM(emArray[i]);
            }
            return tmArray;
        }

        public static AttributeSelectorTM GetAttributeSelectorTM(AttributeSelectorEM em) {
            AttributeSelectorTM tm;
            tm.hp = em.hp;
            tm.hp_ComparisonType = em.hp_ComparisonType;
            tm.hpMax = em.hpMax;
            tm.hpMax_ComparisonType = em.hpMax_ComparisonType;
            tm.mp = em.mp;
            tm.ep_ComparisonType = em.ep_ComparisonType;
            tm.mpMax = em.mpMax;
            tm.mpMax_ComparisonType = em.mpMax_ComparisonType;
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

        public static int[] GetSpeedArray_AnimationCurve(float totalDis, int totalFrame, AnimationCurve curve) {
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

        // TODO 根据曲线计算出移动方向
        public static Vector3Int[] GetDirectionArray_AnimationCurve(int totalFrame, AnimationCurve curve) {
            Vector3Int[] moveDirArray = new Vector3Int[totalFrame];
            for (int i = 0; i < totalFrame; i++) {
                moveDirArray[i] = Vector3Int.right;
            }
            return moveDirArray;
        }

        public static KeyframeTM[] GetKeyframeTMArray(AnimationCurve curve) {
            var keyframeArray = curve.keys;
            var len = keyframeArray.Length;
            var keyFrameArray = new KeyframeTM[len];
            for (int i = 0; i < len; i++) {
                keyFrameArray[i] = new KeyframeTM(keyframeArray[i]);
            }
            return keyFrameArray;
        }

        #endregion

        #region [MoveCurve]

        public static MoveCurveTM GetMoveCurveTM(MoveCurveEM em) {
            var moveDistance_cm = GetInt_Expand100(em.moveDistance);
            var moveTotalFrame = em.moveTotalFrame;
            var disCurve = em.disCurve;

            MoveCurveTM tm;
            tm.moveDistance_cm = moveDistance_cm;
            tm.moveTotalFrame = moveTotalFrame;
            tm.disCurve_KeyframeTMArray = GetKeyframeTMArray(disCurve);
            tm.moveSpeedArray = GetSpeedArray_AnimationCurve(moveDistance_cm, moveTotalFrame, disCurve);
            tm.moveDirArray = GetDirectionArray_AnimationCurve(em.moveTotalFrame, disCurve);

            return tm;
        }

        #endregion

        #region [Misc]

        static string[] GetRelativePathArray<T>(GameObject[] goArray) {
            var len = goArray.Length;
            var pathArray = new string[len];
            for (int i = 0; i < len; i++) {
                var go = goArray[i];
                if (go == null) continue;
                pathArray[i] = GetRelativePath<T>(go.transform);
            }
            return pathArray;
        }

        static string GetRelativePath<T>(Transform begin) {
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

        static int[] GetIntArray_Expand100(float[] vArray) {
            var len = vArray.Length;
            var intArray = new int[len];
            for (int i = 0; i < len; i++) {
                intArray[i] = Mathf.RoundToInt(vArray[i] * 100);
            }
            return intArray;
        }

        static int GetInt_Expand100(float v) {
            return Mathf.RoundToInt(v * 100);
        }

        static Vector3Int GetVector3Int_Expand100(Vector3 v) {
            return new Vector3Int(Mathf.RoundToInt(v.x * 100), Mathf.RoundToInt(v.y * 100), Mathf.RoundToInt(v.z * 100));
        }

        #endregion

    }

}
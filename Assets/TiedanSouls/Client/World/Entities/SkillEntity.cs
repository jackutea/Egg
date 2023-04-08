using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class SkillEntity : IEntity {

        public EntityIDComponent IDCom { get; private set; }

        // - 技能类型
        SkillType skillType;
        public SkillType SkillType => this.skillType;
        public void SetSkillType(SkillType value) => this.skillType = value;

        // - 原始技能
        int originalSkillTypeID;
        public int OriginalSkillTypeID => this.originalSkillTypeID;
        public void SetOriginalSkillTypeID(int value) => this.originalSkillTypeID = value;

        // - 组合技
        SkillCancelModel[] comboSkillCancelModelArray;
        public SkillCancelModel[] ComboSkillCancelModelArray => this.comboSkillCancelModelArray;
        public void SetComboSkillCancelModelArray(SkillCancelModel[] value) => this.comboSkillCancelModelArray = value;

        // - 连招技
        SkillCancelModel[] linkSkillCancelModelArray;
        public SkillCancelModel[] LinkSkillCancelModelArray => this.linkSkillCancelModelArray;
        public void SetLinkSkillCancelModelArray(SkillCancelModel[] value) => this.linkSkillCancelModelArray = value;

        // - 碰撞器
        EntityColliderTriggerModel[] entityColliderTriggerModelArray;
        public EntityColliderTriggerModel[] EntityColliderTriggerModelArray => this.entityColliderTriggerModelArray;
        public void SetEntityColliderTriggerModelArray(EntityColliderTriggerModel[] value) => this.entityColliderTriggerModelArray = value;

        // - 技能效果器
        SkillEffectorModel[] skillEffectorModelArray;
        public void SetSkillEffectorModelArray(SkillEffectorModel[] value) => this.skillEffectorModelArray = value;

        // - 位移曲线模型
        SkillMoveCurveModel[] skillMoveCurveModelArray;
        public void SetSkillMoveCurveModelArray(SkillMoveCurveModel[] value) => this.skillMoveCurveModelArray = value;
        public bool TryGet_ValidSkillMoveCurveModel(out SkillMoveCurveModel skillMoveCurveModel) {
            skillMoveCurveModel = default;
            if (curFrame < 0) return false;
            var len = skillMoveCurveModelArray?.Length;
            for (int i = 0; i < len; i++) {
                var model = skillMoveCurveModelArray[i];
                if (model.startFrame != curFrame) continue;
                skillMoveCurveModel = model;
                return true;
            }

            return false;
        }

        // - 表现层
        string weaponAnimName;
        public string WeaponAnimName => this.weaponAnimName;
        public void SetWeaponAnimName(string value) => this.weaponAnimName = value;

        // - 生命周期
        int maintainFrame;
        public void SetMaintainFrame(int value) => this.maintainFrame = value;

        int curFrame;
        public int CurFrame => this.curFrame;

        public SkillEntity() {
            IDCom = new EntityIDComponent();
            IDCom.SetEntityType(EntityType.Skill);
        }

        public void Reset() {
            curFrame = -1;
            ResetAllColliderModel();
        }

        public void ResetAllColliderModel() {
            var colliderTriggerCount = entityColliderTriggerModelArray?.Length;
            for (int i = 0; i < colliderTriggerCount; i++) {
                var entityColliderTriggerModel = entityColliderTriggerModelArray[i];
                var entityColliderModelArray = entityColliderTriggerModel.entityColliderArray;
                var colliderCount = entityColliderModelArray.Length;
                for (int j = 0; j < colliderCount; j++) {
                    var entityColliderModel = entityColliderModelArray[j];
                    entityColliderModel.transform.position = entityColliderModel.ColliderModel.localPos;
                    entityColliderModel.transform.rotation = Quaternion.Euler(0, 0, entityColliderModel.ColliderModel.localAngleZ);
                    var localScale = entityColliderModel.ColliderModel.localScale;
                    entityColliderModel.transform.localScale = localScale;
                    entityColliderModel.Deactivate();
                }
            }
        }

        #region [Component Wrapper]

        public void SetFather(in EntityIDArgs father) {
            IDCom.SetFather(father);
            var len = entityColliderTriggerModelArray.Length;
            var idArgs = IDCom.ToArgs();
            for (int i = 0; i < len; i++) {
                var triggerModel = entityColliderTriggerModelArray[i];
                var colliderModelArray = triggerModel.entityColliderArray;
                var colliderCount = colliderModelArray.Length;
                for (int j = 0; j < colliderCount; j++) {
                    var colliderModel = colliderModelArray[j];
                    colliderModel.SetFather(idArgs);
                }
            }
        }

        #endregion

        // TODO: 重构 实体内不做过多逻辑处理
        public bool TryMoveNext(Vector3 rootPos, Quaternion rootRot) {
            if (curFrame > maintainFrame) {
                Reset();
                return false;
            }

            curFrame++;

            // 碰撞盒控制
            Foreach_CollisionTrigger(TriggerBegin, Triggering, TriggerEnd);
            return true;

            void TriggerBegin(EntityColliderTriggerModel triggerModel) => ActivateAllColliderModel(triggerModel, true);
            void Triggering(EntityColliderTriggerModel triggerModel) => ActivateAllColliderModel(triggerModel, true);
            void TriggerEnd(EntityColliderTriggerModel triggerModel) => ActivateAllColliderModel(triggerModel, false);
            void ActivateAllColliderModel(EntityColliderTriggerModel triggerModel, bool active) {
                var entityColliderModelArray = triggerModel.entityColliderArray;
                if(entityColliderModelArray == null) return;
                for (int i = 0; i < entityColliderModelArray.Length; i++) {
                    var entityColliderModel = entityColliderModelArray[i];
                    entityColliderModel.transform.position = rootPos + rootRot * entityColliderModel.ColliderModel.localPos;
                    entityColliderModel.transform.rotation = rootRot * Quaternion.Euler(0, 0, entityColliderModel.ColliderModel.localAngleZ);
                    if (active) entityColliderModel.Activate();
                    else entityColliderModel.Deactivate();
                }
            }
        }

        void Foreach_CollisionTrigger(
            Action<EntityColliderTriggerModel> action_triggerBegin,
            Action<EntityColliderTriggerModel> action_triggering,
            Action<EntityColliderTriggerModel> action_triggerEnd) {
            if (entityColliderTriggerModelArray != null) {
                for (int i = 0; i < entityColliderTriggerModelArray.Length; i++) {
                    EntityColliderTriggerModel model = entityColliderTriggerModelArray[i];
                    var triggerStatus = model.GetTriggerState(curFrame);
                    if (triggerStatus == TriggerState.None) continue;
                    if (triggerStatus == TriggerState.Enter) action_triggerBegin(model);
                    else if (triggerStatus == TriggerState.Stay) action_triggering(model);
                    else if (triggerStatus == TriggerState.Exit) action_triggerEnd(model);
                }
            }
        }

        public void Foreach_CancelModel_Linked_InCurrentFrame(Action<SkillCancelModel> action) {
            if (linkSkillCancelModelArray != null) {
                for (int i = 0; i < linkSkillCancelModelArray.Length; i++) {
                    SkillCancelModel model = linkSkillCancelModelArray[i];
                    if (model.IsInTriggeringFrame(curFrame)) action(model);
                }
            }
        }

        public void Foreach_CancelModel_Combo_InCurrentFrame(Action<SkillCancelModel> action) {
            if (comboSkillCancelModelArray != null) {
                for (int i = 0; i < comboSkillCancelModelArray.Length; i++) {
                    SkillCancelModel model = comboSkillCancelModelArray[i];
                    if (model.IsInTriggeringFrame(curFrame)) action(model);
                }
            }
        }

        public bool TryGet_ValidCollisionTriggerModel(out EntityColliderTriggerModel collisionTriggerModel) {
            if (entityColliderTriggerModelArray != null) {
                for (int i = 0; i < entityColliderTriggerModelArray.Length; i++) {
                    EntityColliderTriggerModel model = entityColliderTriggerModelArray[i];
                    var triggerStatus = model.GetTriggerState(curFrame);
                    if (triggerStatus != TriggerState.None) {
                        collisionTriggerModel = model;
                        return true;
                    }
                }
            }
            collisionTriggerModel = default;
            return false;
        }

        public bool TryGet_ValidSkillEffectorModel(out SkillEffectorModel effectorModel) {
            if (skillEffectorModelArray != null) {
                for (int i = 0; i < skillEffectorModelArray.Length; i++) {
                    SkillEffectorModel model = skillEffectorModelArray[i];
                    if (model.effectorTypeID == 0) continue;
                    if (model.triggerFrame == curFrame) {
                        effectorModel = model;
                        return true;
                    }
                }
            }
            effectorModel = default;
            return false;
        }

    }

}
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
        CollisionTriggerModel[] collisionTriggerArray;
        public CollisionTriggerModel[] CollisionTriggerArray => this.collisionTriggerArray;
        public void SetCollisionTriggerArray(CollisionTriggerModel[] value) => this.collisionTriggerArray = value;

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
            var colliderTriggerCount = collisionTriggerArray?.Length;
            for (int i = 0; i < colliderTriggerCount; i++) {
                var colliderTrigger = collisionTriggerArray[i];
                var colliderModelArray = colliderTrigger.colliderModelArray;
                var colliderCount = colliderModelArray.Length;
                for (int j = 0; j < colliderCount; j++) {
                    var colliderModel = colliderModelArray[j];
                    colliderModel.transform.position = colliderModel.LocalPos;
                    colliderModel.transform.rotation = Quaternion.Euler(0, 0, colliderModel.LocalAngleZ);
                    var size = colliderModel.Size;
                    colliderModel.transform.localScale = size;
                    colliderModel.Deactivate();
                }
            }
        }

        #region [Component Wrapper]

        public void SetFather(in EntityIDArgs father) {
            IDCom.SetFather(father);
            var len = collisionTriggerArray.Length;
            var idArgs = IDCom.ToArgs();
            for (int i = 0; i < len; i++) {
                var triggerModel = collisionTriggerArray[i];
                var colliderModelArray = triggerModel.colliderModelArray;
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

            #region [内部方法]
            void TriggerBegin(CollisionTriggerModel triggerModel) => ActivateAllColliderModel(triggerModel, true);
            void Triggering(CollisionTriggerModel triggerModel) => ActivateAllColliderModel(triggerModel, true);
            void TriggerEnd(CollisionTriggerModel triggerModel) => ActivateAllColliderModel(triggerModel, false);
            void ActivateAllColliderModel(CollisionTriggerModel triggerModel, bool active) {
                var colliderCount = triggerModel.colliderModelArray;
                var colliderModelArray = triggerModel.colliderModelArray;
                for (int i = 0; i < colliderCount.Length; i++) {
                    var colliderModel = colliderCount[i];
                    colliderModel.transform.position = rootPos + rootRot * colliderModel.LocalPos;
                    colliderModel.transform.rotation = rootRot * colliderModel.LocalRot;
                    if (active) colliderModel.Activate();
                    else colliderModel.Deactivate();
                }
            }
            #endregion
        }

        void Foreach_CollisionTrigger(
            Action<CollisionTriggerModel> action_triggerBegin,
            Action<CollisionTriggerModel> action_triggering,
            Action<CollisionTriggerModel> action_triggerEnd) {
            if (collisionTriggerArray != null) {
                for (int i = 0; i < collisionTriggerArray.Length; i++) {
                    CollisionTriggerModel model = collisionTriggerArray[i];
                    var triggerStatus = model.GetTriggerStatus(curFrame);
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

        public bool TryGet_ValidCollisionTriggerModel(out CollisionTriggerModel collisionTriggerModel) {
            if (collisionTriggerArray != null) {
                for (int i = 0; i < collisionTriggerArray.Length; i++) {
                    CollisionTriggerModel model = collisionTriggerArray[i];
                    var triggerStatus = model.GetTriggerStatus(curFrame);
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
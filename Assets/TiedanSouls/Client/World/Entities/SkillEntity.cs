using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class SkillEntity : IEntity {

        IDComponent idCom;
        public IDComponent IDCom => idCom;

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

        // - 打击力度
        HitPowerModel[] hitPowerArray;
        public void SetHitPowerArray(HitPowerModel[] value) => this.hitPowerArray = value;

        // - 碰撞器
        CollisionTriggerModel[] collisionTriggerArray;
        public CollisionTriggerModel[] CollisionTriggerArray => this.collisionTriggerArray;
        public void SetCollisionTriggerArray(CollisionTriggerModel[] value) => this.collisionTriggerArray = value;

        // - 表现层
        string weaponAnimName;
        public string WeaponAnimName => this.weaponAnimName;
        public void SetWeaponAnimName(string value) => this.weaponAnimName = value;

        // - 生命周期
        int startFrame;
        public int StartFrame => this.startFrame;
        public void SetStartFrame(int value) => this.startFrame = value;

        int endFrame;
        public int EndFrame => this.endFrame;
        public void SetEndFrame(int value) => this.endFrame = value;

        int curFrame;

        public SkillEntity() {
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Skill);
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
                    colliderModel.transform.localScale = new Vector3(size.x, size.y, 1);
                    colliderModel.Deactivate();
                }
            }
        }

        public bool TryMoveNext(Vector3 rootPos, Quaternion rootRot) {
            if (curFrame > endFrame) {
                Reset();
                return false;
            }

            curFrame++;

            // 碰撞盒控制
            Foreach_CollisionTrigger(TriggerBegin, TriggerEnd, Triggering);
            return true;

            #region [内部方法]
            void TriggerBegin(CollisionTriggerModel triggerModel) {
            }

            void TriggerEnd(CollisionTriggerModel triggerModel) {
                var colliderCount = triggerModel.colliderModelArray;
                for (int i = 0; i < colliderCount.Length; i++) {
                    var colliderModel = colliderCount[i];
                    colliderModel.Deactivate();
                }
            }

            void Triggering(CollisionTriggerModel triggerModel) {
                var colliderCount = triggerModel.colliderModelArray;
                var colliderModelArray = triggerModel.colliderModelArray;
                for (int i = 0; i < colliderCount.Length; i++) {
                    var colliderModel = colliderCount[i];
                    colliderModel.transform.position = rootPos + rootRot * colliderModel.LocalPos;
                    colliderModel.transform.rotation = rootRot * colliderModel.LocalRot;
                    colliderModel.Activate();
                }
            }
            #endregion
        }

        void Foreach_CollisionTrigger(
            Action<CollisionTriggerModel> action_triggerBegin,
            Action<CollisionTriggerModel> action_triggerEnd,
            Action<CollisionTriggerModel> action_triggering) {
            if (collisionTriggerArray != null) {
                for (int i = 0; i < collisionTriggerArray.Length; i++) {
                    CollisionTriggerModel model = collisionTriggerArray[i];
                    if (!IsBetweenStartAndEnd(curFrame, model.startFrame, model.endFrame)) {
                        continue;
                    }
                    action_triggering?.Invoke(model);
                    if (model.startFrame == curFrame) action_triggerBegin?.Invoke(model);
                    else if (model.endFrame == curFrame) action_triggerEnd?.Invoke(model);
                }
            }
        }

        public void Foreach_CancelModel_Link_InCurrentFrame(Action<SkillCancelModel> action) {
            if (linkSkillCancelModelArray != null) {
                for (int i = 0; i < linkSkillCancelModelArray.Length; i++) {
                    SkillCancelModel model = linkSkillCancelModelArray[i];
                    if (IsBetweenStartAndEnd(curFrame, model.startFrame, model.endFrame)) {
                        action(model);
                    }
                }
            }
        }

        public void Foreach_CancelModel_Combo_InCurrentFrame(Action<SkillCancelModel> action) {
            if (comboSkillCancelModelArray != null) {
                for (int i = 0; i < comboSkillCancelModelArray.Length; i++) {
                    SkillCancelModel model = comboSkillCancelModelArray[i];
                    if (IsBetweenStartAndEnd(curFrame, model.startFrame, model.endFrame)) {
                        action(model);
                    }
                }
            }
        }

        public void TryGet_HitPower_InCurrentFrame(Action<HitPowerModel> action) {
            if (hitPowerArray != null) {
                for (int i = 0; i < hitPowerArray.Length; i++) {
                    HitPowerModel model = hitPowerArray[i];
                    if (IsBetweenStartAndEnd(curFrame, model.startFrame, model.endFrame)) {
                        action(model);
                        return;
                    }
                }
            }
        }

        bool IsBetweenStartAndEnd(int frame, int start, int end) {
            return frame >= start && frame <= end;
        }

    }

}
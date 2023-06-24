using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class SkillEntity {

        EntityIDComponent idCom;
        public EntityIDComponent IDCom => idCom;

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

        // - 效果器组
        EffectorTriggerModel[] skillEffectorModelArray;
        public void SetSkillEffectorModelArray(EffectorTriggerModel[] value) => this.skillEffectorModelArray = value;

        // - 技能位移组
        SkillMoveCurveModel[] skillMoveCurveModelArray;
        public void SetSkillMoveCurveModelArray(SkillMoveCurveModel[] value) => this.skillMoveCurveModelArray = value;

        // - 角色召唤组
        SkillSummonModel[] roleSummonModelArray;
        public void SetRoleSummonModelArray(SkillSummonModel[] value) => this.roleSummonModelArray = value;

        // - 弹幕生成组
        ProjectileCtorModel[] projectileCtorModelArray;
        public void SetProjectileCtorModelArray(ProjectileCtorModel[] value) => this.projectileCtorModelArray = value;

        // - Buff附加组
        BuffAttachModel[] buffAttachModelArray;
        public void SetBuffAttachModelArray(BuffAttachModel[] value) => this.buffAttachModelArray = value;

        // - 碰撞器组
        ColliderToggleModel[] entityColliderTriggerModelArray;
        public ColliderToggleModel[] EntityColliderTriggerModelArray => this.entityColliderTriggerModelArray;
        public void SetEntityColliderTriggerModelArray(ColliderToggleModel[] value) => this.entityColliderTriggerModelArray = value;

        // - 表现层
        string weaponAnimName;
        public string WeaponAnimName => this.weaponAnimName;
        public void SetWeaponAnimName(string value) => this.weaponAnimName = value;

        // - 生命周期
        int totalFrame;
        public int TotalFrame => this.totalFrame;
        public void SetTotalFrame(int value) => this.totalFrame = value;

        int curFrame;
        public int CurFrame => this.curFrame;
        public void SetCurFrame(int value) => this.curFrame = value;

        public SkillEntity() {
            idCom = new EntityIDComponent();
            idCom.SetEntityType(EntityType.Skill);
            idCom.SetHolderPtr(this);
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
                    entityColliderModel.Deactivate();
                }
            }
        }

        public void SetFather(in EntityIDComponent father) {
            IDCom.SetFather(father);
        }

        public bool TryApplyFrame(Vector3 rootPos, Quaternion rootRot, int frame) {
            if (frame > totalFrame) {
                return false;
            }

            // 碰撞盒控制
            if (entityColliderTriggerModelArray != null) {
                for (int i = 0; i < entityColliderTriggerModelArray.Length; i++) {
                    ColliderToggleModel model = entityColliderTriggerModelArray[i];
                    var triggerStatus = model.GetToggleState(frame);
                    if (triggerStatus == ToggleState.None) {
                        continue;
                    }
                    if (triggerStatus == ToggleState.Enter) {
                        ActivateAllColliderModel(model, true);
                    } else if (triggerStatus == ToggleState.Stay) {
                        ActivateAllColliderModel(model, true);
                    } else if (triggerStatus == ToggleState.Exit) {
                        ActivateAllColliderModel(model, false);
                    }
                }
            }

            void ActivateAllColliderModel(ColliderToggleModel triggerModel, bool active) {
                var entityColliderModelArray = triggerModel.entityColliderArray;
                if (entityColliderModelArray == null) return;
                for (int i = 0; i < entityColliderModelArray.Length; i++) {
                    var entityColliderModel = entityColliderModelArray[i];
                    entityColliderModel.SetActive(active);
                    if (active) {
                        entityColliderModel.transform.position = rootPos + rootRot * entityColliderModel.ColliderModel.localPos;
                        entityColliderModel.transform.rotation = rootRot * Quaternion.Euler(0, 0, entityColliderModel.ColliderModel.localAngleZ);
                        entityColliderModel.transform.localScale = entityColliderModel.ColliderModel.localScale;
                    }
                }
            }
            return true;
        }

        public bool TryGetSkillMoveCurveModel(int frame, out SkillMoveCurveModel skillMoveCurveModel) {
            if (frame < 0) {
                skillMoveCurveModel = default;
                return false;
            }

            var len = skillMoveCurveModelArray?.Length;
            for (int i = 0; i < len; i++) {
                var model = skillMoveCurveModelArray[i];
                var startFrame = model.triggerFrame;
                var endFrame = startFrame + model.moveCurveModel.moveDirArray.Length - 1;

                if (startFrame <= frame && frame <= endFrame) {
                    skillMoveCurveModel = model;
                    return true;
                }
            }

            skillMoveCurveModel = default;
            return false;
        }

        public bool HasSkillMoveCurveModel(int frame) {
            if (frame < 0) {
                return false;
            }

            var len = skillMoveCurveModelArray?.Length;
            for (int i = 0; i < len; i++) {
                var model = skillMoveCurveModelArray[i];
                var startFrame = model.triggerFrame;
                var endFrame = startFrame + model.moveCurveModel.moveDirArray.Length - 1;

                if (startFrame <= frame && frame <= endFrame) {
                    return true;
                }
            }

            return false;
        }

        public bool TryGet_ValidCollisionTriggerModel(out ColliderToggleModel collisionTriggerModel, int frame = -1) {
            frame = frame == -1 ? curFrame : frame;
            if (entityColliderTriggerModelArray != null) {
                for (int i = 0; i < entityColliderTriggerModelArray.Length; i++) {
                    ColliderToggleModel model = entityColliderTriggerModelArray[i];
                    var triggerStatus = model.GetToggleState(frame);
                    if (triggerStatus != ToggleState.None) {
                        collisionTriggerModel = model;
                        return true;
                    }
                }
            }
            collisionTriggerModel = default;
            return false;
        }

        public bool TryGet_ValidSkillEffectorModel(out EffectorTriggerModel effectorModel, int frame = -1) {
            frame = frame == -1 ? curFrame : frame;
            if (skillEffectorModelArray != null) {
                for (int i = 0; i < skillEffectorModelArray.Length; i++) {
                    EffectorTriggerModel model = skillEffectorModelArray[i];
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

        public bool TryGet_ValidRoleSummonModel(out SkillSummonModel roleSummonModel, int frame = -1) {
            frame = frame == -1 ? curFrame : frame;
            if (roleSummonModelArray != null) {
                for (int i = 0; i < roleSummonModelArray.Length; i++) {
                    SkillSummonModel model = roleSummonModelArray[i];
                    if (model.triggerFrame == curFrame) {
                        roleSummonModel = model;
                        return true;
                    }
                }
            }
            roleSummonModel = default;
            return false;
        }

        public bool TryGet_ValidProjectileCtorModel(out ProjectileCtorModel projectileCtorModel, int frame = -1) {
            frame = frame == -1 ? curFrame : frame;
            if (projectileCtorModelArray != null) {
                for (int i = 0; i < projectileCtorModelArray.Length; i++) {
                    ProjectileCtorModel model = projectileCtorModelArray[i];
                    if (model.triggerFrame == curFrame) {
                        projectileCtorModel = model;
                        return true;
                    }
                }
            }
            projectileCtorModel = default;
            return false;
        }

        public bool TryGet_ValidBuffAttachModel(out BuffAttachModel buffAttachModel, int frame = -1) {
            frame = frame == -1 ? curFrame : frame;
            if (buffAttachModelArray != null) {
                for (int i = 0; i < buffAttachModelArray.Length; i++) {
                    BuffAttachModel model = buffAttachModelArray[i];
                    if (model.triggerFrame == curFrame) {
                        buffAttachModel = model;
                        return true;
                    }
                }
            }
            buffAttachModel = default;
            return false;
        }

        public void Foreach_CancelModel_Linked(Action<SkillCancelModel> action, int frame = -1) {
            frame = frame == -1 ? curFrame : frame;
            if (linkSkillCancelModelArray != null) {
                for (int i = 0; i < linkSkillCancelModelArray.Length; i++) {
                    SkillCancelModel model = linkSkillCancelModelArray[i];
                    if (model.IsInTriggeringFrame(curFrame)) action(model);
                }
            }
        }

        public void Foreach_CancelModel_Combo(Action<SkillCancelModel> action, int frame = -1) {
            frame = frame == -1 ? curFrame : frame;
            if (comboSkillCancelModelArray != null) {
                for (int i = 0; i < comboSkillCancelModelArray.Length; i++) {
                    SkillCancelModel model = comboSkillCancelModelArray[i];
                    if (model.IsInTriggeringFrame(curFrame)) action(model);
                }
            }
        }

        public bool IsKeyFrame(int frame) {
            return TryGet_ValidCollisionTriggerModel(out _, frame)
            || TryGet_ValidSkillEffectorModel(out _, frame)
            || TryGet_ValidRoleSummonModel(out _, frame)
            || TryGet_ValidProjectileCtorModel(out _, frame)
            || TryGet_ValidBuffAttachModel(out _, frame);
        }

    }

}
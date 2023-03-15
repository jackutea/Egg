using System;
using TiedanSouls.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillEntity {

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
        SkillCancelModel[] skillCancelModelArray;
        public SkillCancelModel[] SkillCancelModelArray => this.skillCancelModelArray;
        public void SetSkillCancelModelArray(SkillCancelModel[] value) => this.skillCancelModelArray = value;

        // - 打击力度
        HitPowerModel[] hitPowerArray;
        public void SetHitPowerArray(HitPowerModel[] value) => this.hitPowerArray = value;

        // - 碰撞器
        CollisionTriggerModel[] collisionTriggerArray;
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
        }

        public void MoveNext() {
            if (curFrame > endFrame) {
                curFrame = -1;
                return;
            }

            curFrame++;

            // 碰撞盒控制
            Foreach_CollisionTrigger(
                (model) => {
                    var colliderCount = model.colliderGOArray;
                    for (int i = 0; i < colliderCount.Length; i++) {
                        var colliderGO = colliderCount[i];
                        colliderGO.SetActive(true);
                    }
                },
                (model) => {
                    var colliderCount = model.colliderGOArray;
                    for (int i = 0; i < colliderCount.Length; i++) {
                        var colliderGO = colliderCount[i];
                        colliderGO.SetActive(false);
                    }
                }
            );
        }

        void Foreach_CollisionTrigger(Action<CollisionTriggerModel> action_activated, Action<CollisionTriggerModel> action_not) {
            if (collisionTriggerArray != null) {
                for (int i = 0; i < collisionTriggerArray.Length; i++) {
                    CollisionTriggerModel model = collisionTriggerArray[i];
                    if (model.startFrame == curFrame) action_activated(model);
                    else action_not(model);
                }
            }
        }

        public void Foreach_CancelModel_InCurrentFrame(Action<SkillCancelModel> action) {
            if (skillCancelModelArray != null) {
                for (int i = 0; i < skillCancelModelArray.Length; i++) {
                    SkillCancelModel model = skillCancelModelArray[i];
                    if (model.startFrame == curFrame) {
                        action(model);
                    }
                }
            }
        }

        public void TryGet_HitPower_InCurrentFrame(Action<HitPowerModel> action) {
            if (hitPowerArray != null) {
                for (int i = 0; i < hitPowerArray.Length; i++) {
                    HitPowerModel model = hitPowerArray[i];
                    if (model.startFrame == curFrame) {
                        action(model);
                        return;
                    }
                }
            }
        }

    }

}
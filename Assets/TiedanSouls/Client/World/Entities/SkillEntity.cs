using TiedanSouls.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillEntity {

        IDComponent idCom;
        public IDComponent IDCom => idCom;

        // - 技能类型
        SkillType skillType;
        public SkillType SkillType => this.skillType;
        public void SetSkillType(SkillType value) => this.skillType = value;

        // - 生命周期
        int startFrame;
        public int StartFrame => this.startFrame;
        public void SetStartFrame(int value) => this.startFrame = value;

        int endFrame;
        public int EndFrame => this.endFrame;
        public void SetEndFrame(int value) => this.endFrame = value;

        public int curFrame;

        // - 原始技能
        int originalSkillTypeID;
        public int OriginalSkillTypeID => this.originalSkillTypeID;
        public void SetOriginalSkillTypeID(int value) => this.originalSkillTypeID = value;

        // - 连招技能
        SkillCancelModel[] comboSkillCancelModelArray;
        public SkillCancelModel[] ComboSkillCancelModelArray => this.comboSkillCancelModelArray;
        public void SetComboSkillCancelModelArray(SkillCancelModel[] value) => this.comboSkillCancelModelArray = value;

        // - 可强制取消技能
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

        public SkillEntity() {
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Skill);
        }

        public void Reset() {
            curFrame = 0;
        }

        public bool TryGet_AllCurrentCancelModel(out SkillCancelModel[] cancelModel) {
            cancelModel = default;
            if (skillCancelModelArray == null) {
                return false;
            }
            for (int i = 0; i < skillCancelModelArray.Length; i++) {
                var model = skillCancelModelArray[i];
                if (curFrame >= model.startFrame && curFrame <= model.endFrame) {
                    cancelModel = model;
                    return true;
                }
            }
            return false;
        }

    }

}
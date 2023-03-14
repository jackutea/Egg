using TiedanSouls.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillEntity {

        IDComponent idCom;
        public IDComponent IDCom => idCom;

        // - 生命周期
        int startFrame;
        public int StartFrame => this.startFrame;
        public void SetStartFrame(int value) => this.startFrame = value;

        int endFrame;
        public int EndFrame => this.endFrame;
        public void SetEndFrame(int value) => this.endFrame = value;

        // - 原始技能
        int originalSkillTypeID;
        public int OriginalSkillTypeID => this.originalSkillTypeID;
        public void SetOriginalSkillTypeID(int value) => this.originalSkillTypeID = value;

        // - 连招技能
        SkillCancelModel[] comboSkillTypeIDArray;
        public SkillCancelModel[] ComboSkillTypeIDArray => this.comboSkillTypeIDArray;
        public void SetComboSkillTypeIDArray(SkillCancelModel[] value) => this.comboSkillTypeIDArray = value;

        // - 可强制取消技能
        SkillCancelModel[] cancelSkillTypeIDArray;
        public SkillCancelModel[] CancelSkillTypeIDArray => this.cancelSkillTypeIDArray;
        public void SetCancelSkillTypeIDArray(SkillCancelModel[] value) => this.cancelSkillTypeIDArray = value;

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

    }

}
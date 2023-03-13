using TiedanSouls.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillEntity {

        IDComponent idCom;
        public IDComponent IDCom => idCom;

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

        // - Hit Power
        HitPowerModel[] hitPowerArray;
        public void SetHitPowerArray(HitPowerModel[] value) => this.hitPowerArray = value;

        // - Collision Trigger
        CollisionTriggerModel[] collisionTriggerArray;
        public void SetCollisionTriggerArray(CollisionTriggerModel[] value) => this.collisionTriggerArray = value;

        // - Renderer
        string weaponAnimName;
        public string WeaponAnimName => this.weaponAnimName;
        public void SetWeaponAnimName(string value) => this.weaponAnimName = value;

        public SkillEntity() {
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Skill);
        }

    }

}
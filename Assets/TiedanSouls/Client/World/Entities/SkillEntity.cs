using TiedanSouls.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillEntity {

        IDComponent idCom;
        public IDComponent IDCom => idCom;

        // - Combo
        int originalSkillTypeID;
        public int OriginalSkillTypeID => this.originalSkillTypeID;
        public void SetOriginalSkillTypeID(int value) => this.originalSkillTypeID = value;

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
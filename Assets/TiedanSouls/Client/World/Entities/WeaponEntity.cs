using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class WeaponEntity : IEntity {

        #region [组件]

        public EntityIDComponent IDCom { get; private set; }
        public WeaponAttributeComponent AttributeCom { get; private set; }
        public WeaponFSMComponent FSMCom { get; private set; }
        public BuffSlotComponent BuffSlotCom { get; private set; }

        #endregion

        WeaponType weaponType;
        public WeaponType WeaponType => weaponType;
        public void SetWeaponType(WeaponType weaponType) => this.weaponType = weaponType;

        int skillMeleeTypeID;
        public int SkillMeleeTypeID => skillMeleeTypeID;
        public void SetSkillMeleeTypeID(int skillMeleeTypeID) => this.skillMeleeTypeID = skillMeleeTypeID;

        int skillHoldMeleeTypeID;
        public int SkillHoldMeleeTypeID => skillHoldMeleeTypeID;
        public void SetSkillHoldMeleeTypeID(int skillHoldMeleeTypeID) => this.skillHoldMeleeTypeID = skillHoldMeleeTypeID;

        int skillSpecMeleeTypeID;
        public int SkillSpecMeleeTypeID => skillSpecMeleeTypeID;
        public void SetSkillSpecMeleeTypeID(int skillSpecMeleeTypeID) => this.skillSpecMeleeTypeID = skillSpecMeleeTypeID;

        // ==== Renderer ====
        GameObject mod;
        public GameObject Mod => mod;

        Animator animator;

        public WeaponEntity() {
            this.IDCom = new EntityIDComponent();
            this.IDCom.SetEntityType(EntityType.Weapon);
            this.AttributeCom = new WeaponAttributeComponent();
            this.FSMCom = new WeaponFSMComponent();
            this.BuffSlotCom = new BuffSlotComponent();
        }

        // ==== Renderer ====
        public void SetMod(GameObject mod) {
            this.mod = mod;
            animator = mod.GetComponent<Animator>();
        }

        public void PlayAnim(string animName) {
            animator.Play(animName);
        }

    }

}
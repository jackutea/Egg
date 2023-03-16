using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class WeaponEntity {

        WeaponType weaponType;
        public WeaponType WeaponType => weaponType;
        public void SetWeaponType(WeaponType weaponType) => this.weaponType = weaponType;

        int typeID;
        public int TypeID => typeID;
        public void SetTypeID(int typeID) => this.typeID = typeID;

        public int atk;
        public int def;
        public int crit;

        public int skillMeleeTypeID;
        public int skillHoldMeleeTypeID;
        public int skillSpecMeleeTypeID;

        // ==== Renderer ====
        GameObject mod;
        public GameObject Mod => mod;

        Animator animator;

        public WeaponEntity() { }

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
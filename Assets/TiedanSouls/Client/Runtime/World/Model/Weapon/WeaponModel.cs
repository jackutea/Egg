using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class WeaponModel {

        public WeaponType weaponType;
        public int typeID;

        public int atk;
        public int def;
        public int crit;

        public int skillorMeleeTypeID;
        public int skillorHoldMeleeTypeID;
        public int skillorSpecMeleeTypeID;

        // ==== Renderer ====
        GameObject mod;
        public GameObject Mod => mod;

        Animator animator;

        public WeaponModel() {}

        // ==== Renderer ====
        public void SetMod(GameObject mod) {
            this.mod = mod.gameObject;
            animator = mod.GetComponent<Animator>();
        }

        public void PlayAnim(string animName) {
            animator.Play(animName);
        }

    }

}
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
        GameObject go;
        public GameObject Go => go;
        Animator animator;

        public WeaponModel() {}

        // ==== Renderer ====
        public void SetMod(GameObject mod) {
            go = mod.gameObject;
            animator = mod.GetComponent<Animator>();
        }

        public void PlayAnim(string animName) {
            animator.Play(animName);
        }

    }

}
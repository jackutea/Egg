using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleModComponent {

        GameObject mod;
        SpriteRenderer sr;
        Animator anim;

        public RoleModComponent() { }

        public void SetMod(GameObject mod) {
            this.mod = mod;
            sr = mod.GetComponent<SpriteRenderer>();
            anim = mod.GetComponent<Animator>();
        }

        public void Anim_PlayIdle() {
            anim.Play("idle");
        }

        public void Anim_PlayBeHurt() {
            anim.Play("be_hurt");
        }

    }

}
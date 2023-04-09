using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RoleRendererComponent {

        GameObject mod;
        public GameObject Mod => mod;

        SpriteRenderer sr;
        Animator anim;

        public RoleRendererComponent() { }

        public void SetMod(GameObject mod) {
            this.mod = mod;
            sr = mod.GetComponentInChildren<SpriteRenderer>();
            anim = mod.GetComponent<Animator>();
        }

        public void Anim_PlayIdle() {
            anim.Play("Idle");
        }

        public void Anim_Play_BeHit() {
            anim.Play("BeHit");
        }

        public void Anim_Play_Dying() {
            anim.Play("Dying");
        }

        public void Anim_SetSpeed(float speed) {
            anim.speed = speed;
        }

    }

}
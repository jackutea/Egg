using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleRendererComponent {

        GameObject mod;
        public GameObject Mod => mod;

        Transform rendererRoot;
        Animator anim;
        SpriteRenderer sr;

        public RoleRendererComponent() {
        }

        public void Inject(Transform rendererRoot) {
            this.rendererRoot = rendererRoot;
        }

        public void Reset() {
        }

        public void Show() {
            rendererRoot.gameObject.SetActive(true);
        }

        public void Hide() {
            rendererRoot.gameObject.SetActive(false);
        }

        public void SetMod(GameObject mod) {
            mod.transform.SetParent(rendererRoot);
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

        public void LerpPosition(Vector3 dstPos, float dt) {
            if (Vector3.Distance(rendererRoot.position, dstPos) < GameCollection.LERP_MIN_DISTANCE) {
                rendererRoot.position = dstPos;
                return;
            }

            var ratio = dt / GameCollection.LERP_DURATION;
            rendererRoot.position = Vector3.Lerp(rendererRoot.position, dstPos, ratio);
        }

        public void LerpRotation(Quaternion dstRot, float dt) {
            mod.transform.rotation = dstRot;
        }

        public void SetPos(Vector3 pos) {
            rendererRoot.position = pos;
        }

        public void SetRotation(Quaternion rot) {
            rendererRoot.rotation = rot;
        }

    }

}
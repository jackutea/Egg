using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RoleRendererComponent {

        GameObject mod;
        public GameObject Mod => mod;

        HUDSlotComponent hudSlotCom;
        public HUDSlotComponent HudSlotCom => hudSlotCom;

        Transform rendererRoot;
        Animator anim;
        SpriteRenderer sr;

        public RoleRendererComponent() {
            hudSlotCom = new HUDSlotComponent();
        }

        public void Inject(Transform rendererRoot) {
            this.rendererRoot = rendererRoot;

            var hudRoot = rendererRoot.Find("hud_root");
            hudSlotCom.Inject(hudRoot);
        }

        public void Reset(float gp, float hp, float hpMax) {
            hudSlotCom.Reset(gp, hp, hpMax);
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

        const float lerpDuration = 0.05f;
        const float minLerpDistance = 0.1f;
        public void LerpPosition(Vector3 dstPos, float dt) {
            if (Vector3.Distance(rendererRoot.position, dstPos) < minLerpDistance) {
                rendererRoot.position = dstPos;
                return;
            }
            
            var ratio = dt / lerpDuration;
            rendererRoot.position = Vector3.Lerp(rendererRoot.position, dstPos, ratio);
        }

        public void LerpRotation(Quaternion dstRot, float dt) {
            rendererRoot.rotation = dstRot;
        }

        public void TickHUD(RoleAttributeComponent attributeCom, float dt) {
            hudSlotCom.Tick(attributeCom, dt);
        }

        public void SetPos(Vector3 pos) {
            rendererRoot.position = pos;
        }

        public void SetRotation(Quaternion rot) {
            rendererRoot.rotation = rot;
        }

    }

}
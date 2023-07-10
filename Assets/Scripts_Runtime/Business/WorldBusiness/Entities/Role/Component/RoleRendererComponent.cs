using UnityEngine;
using TiedanSouls.Generic;
using GameArki.Anymotion;

namespace TiedanSouls.Client.Entities {

    public class RoleRendererComponent {

        GameObject mod;

        Transform rendererRoot;
        AnymotionGo anymotion;

        SpriteRenderer sr;

        public RoleRendererComponent() { }

        public void Inject(Transform rendererRoot) {
            this.rendererRoot = rendererRoot;
        }

        public void Init(AnymotionSO so) {
            AnymotionUtil.InitializeAndPlayDefault(0, anymotion, so);
        }

        public void Tick(float dt) {
            anymotion.Tick(dt);
        }

        public void TearDown() {
            anymotion.TearDown();
        }

        public void Reset() {
        }

        public void ShowRenderer() {
            rendererRoot.gameObject.SetActive(true);
        }

        public void HideRenderer() {
            rendererRoot.gameObject.SetActive(false);
        }

        public void SetMod(GameObject mod) {
            mod.transform.SetParent(rendererRoot);
            this.mod = mod;
            anymotion = mod.GetComponent<AnymotionGo>();
            anymotion.Ctor();
            sr = mod.GetComponentInChildren<SpriteRenderer>();
        }

        public void Anim_Idle() {
            // anymotion.Play(0, "Idle", true);
        }

        public void Anim_Move(float moveMagnitudeSqr) {
            if (moveMagnitudeSqr == 0) {
                anymotion.Play(0, "Idle", false);
            } else {
                anymotion.Play(0, "Walk", false);
            }
        }

        public void Anim_PlaySkill(string skillName) {
            anymotion.Play(0, skillName, true);
        }

        public void Anim_Play_BeHit() {
            anymotion.Play(0, "BeHit", true);
        }

        public void Anim_Play_Dying() {
            anymotion.Play(0, "Dying", true);
        }

        public void Anim_SetSpeed(float speed) {
            anymotion.SetSpeed(0, speed);
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
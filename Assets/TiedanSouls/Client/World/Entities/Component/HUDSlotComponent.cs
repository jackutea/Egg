using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class HUDSlotComponent {

        Transform hudRoot;
        public Transform HudRoot => hudRoot;

        HpBarHUD hpBarHUD;
        public HpBarHUD HpBarHUD => hpBarHUD;

        public HUDSlotComponent() { }

        public void Inject(Transform hudRoot) {
            this.hudRoot = hudRoot;
        }

        public void SetHpBarHUD(HpBarHUD hpBarHUD) {
            this.hpBarHUD = hpBarHUD;
            hpBarHUD.transform.SetParent(hudRoot, false);
        }

        public void Reset() {
            hpBarHUD.gameObject.SetActive(false);
        }

        public void HideHUD() {
            hpBarHUD.gameObject.SetActive(false);
        }

    }

}
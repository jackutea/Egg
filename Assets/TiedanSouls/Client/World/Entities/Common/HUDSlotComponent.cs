using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class HUDSlotComponent {

        Transform hudRoot;
        public Transform HudRoot => hudRoot;

        HPBarHUD hpBarHUD;
        public HPBarHUD HpBarHUD => hpBarHUD;

        DamageFloatTextHUD damageFloatTextHUD;
        public DamageFloatTextHUD DamageFloatTextHUD => damageFloatTextHUD;

        public HUDSlotComponent() { }

        public void Inject(Transform hudRoot) {
            this.hudRoot = hudRoot;
        }

        public void SetHpBarHUD(HPBarHUD hpBarHUD) {
            this.hpBarHUD = hpBarHUD;
            hpBarHUD.transform.SetParent(hudRoot, false);
        }
        
        public void SetDamageFloatTextHUD(DamageFloatTextHUD damageFloatTextHUD) {
            this.damageFloatTextHUD = damageFloatTextHUD;
            damageFloatTextHUD.transform.SetParent(hudRoot, false);
        }
        
        public void Reset() {
        }

        public void HideHUD() {
            hpBarHUD.gameObject.SetActive(false);
        }

        public void ShowHUD() {
            hpBarHUD.gameObject.SetActive(true);
        }

    }

}
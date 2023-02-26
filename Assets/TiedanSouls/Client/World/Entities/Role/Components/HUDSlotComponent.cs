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
            TDLog.Log($"设置血条: hpBarHUD {hpBarHUD}    --- hudRoot{hudRoot}");
        }

    }

}
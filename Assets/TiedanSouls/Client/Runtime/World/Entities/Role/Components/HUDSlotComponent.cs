using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class HUDSlotComponent {

        Transform hudRoot;
        public Transform HudRoot => hudRoot;

        GameObject hpBarHUD;
        public GameObject HpBarHUD => hpBarHUD;

        public HUDSlotComponent () { }

        public void Inject(Transform hudRoot) {
            this.hudRoot = hudRoot;
        }

        public void SetHpBarHUD(GameObject hpBarHUD){
            this.hpBarHUD = hpBarHUD;
        }

    }

}
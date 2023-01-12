using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class HpBarHUDComponent{

        Transform hpBarHUDRoot;
        public Transform HpBarHUDRoot => hpBarHUDRoot;


        public HpBarHUDComponent() { }

        public void Inject(Transform hpBarHUDRoot) {
            this.hpBarHUDRoot = hpBarHUDRoot;
        }

        public void SetHpBarHUD(GameObject HpBarHUD) {
            HpBarHUD.transform.SetParent(hpBarHUDRoot);
            HpBarHUD.transform.localPosition =Vector3.zero;
        }

    }

}
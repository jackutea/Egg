using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TiedanSouls.World.Entities {
    public class HpBarHUD : MonoBehaviour {

        Image hpBarMove;
        public Image HpBarMove => hpBarMove;

        Image hpBarCurrent;
        public Image HpBarCurrent => hpBarCurrent;

        float percent;
        float moveSpeed = 0.03f;

        public void Awake()
        {
            Ctor();
            SetHpBar(50,100);
        }

        public void Ctor() {
            hpBarMove = transform.Find("hud_hp_move").GetComponent<Image>();
            hpBarCurrent = transform.Find("hud_hp_current").GetComponent<Image>();
        }

        public void SetHpBar(int hpCurrent, int hpMax) {
            percent = (float)hpCurrent / hpMax;
            hpBarCurrent.fillAmount = percent;
        }

        public void Tick(float dt) {
            if (hpBarMove.fillAmount!=HpBarCurrent.fillAmount) { 
                if(hpBarMove.fillAmount>HpBarCurrent.fillAmount){
                    hpBarMove.fillAmount -= moveSpeed*dt;
                }
                else{
                    hpBarMove.fillAmount += moveSpeed*dt;
                }
            }
        }
    }
}


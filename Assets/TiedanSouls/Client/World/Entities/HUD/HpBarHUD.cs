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
        float moveSpeed;


        public void Ctor() {
            hpBarMove = transform.Find("hud_hp_move").GetComponent<Image>();
            hpBarCurrent = transform.Find("hud_hp_current").GetComponent<Image>();
        }

        public void SetHpBar(int hpCurrent, int hpMax) {
            percent = (float)hpCurrent / hpMax;
            hpBarCurrent.fillAmount = percent;
        }

        public void Tick(float dt) {
            if (hpBarMove.fillAmount!=hpBarCurrent.fillAmount) { 
                moveSpeed = 2*Mathf.Abs(hpBarMove.fillAmount-hpBarCurrent.fillAmount);
                if(hpBarMove.fillAmount>hpBarCurrent.fillAmount){
                    hpBarMove.fillAmount -= moveSpeed*dt;
                }
                else{
                    hpBarMove.fillAmount += moveSpeed*dt;
                }
            }
        }
    }
}


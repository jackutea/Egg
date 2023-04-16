using UnityEngine;
using UnityEngine.UI;

namespace TiedanSouls.Generic {
    
    public class HPBarHUD : MonoBehaviour {

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

        public void Tick(float dt) {
            if (hpBarMove.fillAmount != hpBarCurrent.fillAmount) {
                moveSpeed = 2 * Mathf.Abs(hpBarMove.fillAmount - hpBarCurrent.fillAmount);
                if (hpBarMove.fillAmount > hpBarCurrent.fillAmount) {
                    hpBarMove.fillAmount -= moveSpeed * dt;
                } else {
                    hpBarMove.fillAmount += moveSpeed * dt;
                }
            }
        }

        public void SetHpBar(float hpCurrent, float hpMax) {
            percent = hpCurrent / hpMax;
            hpBarCurrent.fillAmount = percent;
        }

        public void SetColor(Color color) {
            hpBarCurrent.color = color;
        }

    }
}

using UnityEngine;
using UnityEngine.UI;

namespace TiedanSouls.Generic {

    public class HPBarHUD : MonoBehaviour {

        Transform gpImgTrans;
        Transform hpImgTrans;
        Transform hpFadeImgTrans;

        Image gpImg;
        Image hpImg;
        Image hpFadeImg;

        float gp;
        public void SetGP(float gp) => this.gp = gp;

        float hp;
        public void SetHP(float hp) {
            if (this.hp == hp || this.hp < hp) {
                hpFadeImg.fillAmount = hpImg.fillAmount;
            }

            this.hp = hp;
        }

        float hpMax;
        public void SetHPMax(float hpMax) => this.hpMax = hpMax;

        float hpFadeDuration;
        public void SetHPFadeDuration(float fadeDuration) => this.hpFadeDuration = fadeDuration;

        float time;

        public void Ctor() {
            gpImgTrans = transform.Find("GPImg");
            hpFadeImgTrans = transform.Find("HPFadeImg");
            hpImgTrans = transform.Find("HPImg");

            gpImg = gpImgTrans.GetComponent<Image>();
            hpFadeImg = hpFadeImgTrans.GetComponent<Image>();
            hpImg = hpImgTrans.GetComponent<Image>();

            TDLog.Assert(gpImg != null, "gpImg != null");
            TDLog.Assert(hpFadeImg != null, "hpFadeImg != null");
            TDLog.Assert(hpImg != null, "hpImg != null");

            hpFadeDuration = 0.5f;
        }

        public void Tick(float dt) {
            var total = gp + hp;
            var mom = total > hpMax ? total : hpMax;
            hpImg.fillAmount = hp / mom;
            gpImg.fillAmount = (gp + hp) / mom;
            FadeToCurHP(dt);
        }

        public void SetColor(Color color) {
            hpImg.color = color;
        }

        void FadeToCurHP(float dt) {
            var start = hpFadeImg.fillAmount;
            var end = hpImg.fillAmount;
            var offset = end - start;
            if (Mathf.Abs(offset) < 0.01f) {
                time = 0;
                hpFadeImg.fillAmount = end;
                return;
            }

            time += dt;
            time = Mathf.Min(time, hpFadeDuration);
            hpFadeImg.fillAmount = start + offset * time / hpFadeDuration;
        }

    }
}


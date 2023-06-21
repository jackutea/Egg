using UnityEngine;
using TMPro;

namespace TiedanSouls.Generic {

    public class DamageFloatTextHUD : MonoBehaviour {

        Transform physicalDamageRootTrans;
        Transform[] pdTextTransArray;
        TMP_Text[] pdDmgTextArray;

        Transform magicDamageRootTrans;
        Transform[] mdTextTransArray;
        TMP_Text[] mdDmgTextArray;

        Transform trueDamageRootTrans;
        Transform[] tdTextTransArray;
        TMP_Text[] tdDmgTextArray;

        float[] pdTimeArray;
        float[] pdDurationArray;

        float[] mdTimeArray;
        float[] mdDurationArray;

        float[] tdTimeArray;
        float[] tdDurationArray;

        Color pdColor;
        Color mdColor;
        Color tdColor;

        public void Ctor() {
            physicalDamageRootTrans = transform.Find("PhysicalDamageRoot");
            physicalDamageRootTrans.gameObject.SetActive(true);
            var pdTextCount = physicalDamageRootTrans.childCount;
            pdTextTransArray = new Transform[pdTextCount];
            pdDmgTextArray = new TMP_Text[pdTextCount];
            for (int i = 0; i < pdTextCount; i++) {
                var pdTextTrans = physicalDamageRootTrans.GetChild(i);
                pdTextTransArray[i] = pdTextTrans;
                pdDmgTextArray[i] = pdTextTrans.GetComponent<TMP_Text>();
                pdTextTrans.gameObject.SetActive(false);
            }
            pdTimeArray = new float[pdTextCount];
            pdDurationArray = new float[pdTextCount];

            magicDamageRootTrans = transform.Find("MagicDamageRoot");
            magicDamageRootTrans.gameObject.SetActive(true);
            var mdTextCount = magicDamageRootTrans.childCount;
            mdTextTransArray = new Transform[mdTextCount];
            mdDmgTextArray = new TMP_Text[mdTextCount];
            for (int i = 0; i < mdTextCount; i++) {
                var mdTextTrans = magicDamageRootTrans.GetChild(i);
                mdTextTransArray[i] = mdTextTrans;
                mdDmgTextArray[i] = mdTextTrans.GetComponent<TMP_Text>();
                mdTextTrans.gameObject.SetActive(false);
            }
            mdTimeArray = new float[mdTextCount];
            mdDurationArray = new float[mdTextCount];

            trueDamageRootTrans = transform.Find("TrueDamageRoot");
            trueDamageRootTrans.gameObject.SetActive(true);
            var tdTextCount = trueDamageRootTrans.childCount;
            tdTextTransArray = new Transform[tdTextCount];
            tdDmgTextArray = new TMP_Text[tdTextCount];
            for (int i = 0; i < tdTextCount; i++) {
                var tdTextTrans = trueDamageRootTrans.GetChild(i);
                tdTextTransArray[i] = tdTextTrans;
                tdDmgTextArray[i] = tdTextTrans.GetComponent<TMP_Text>();
                tdTextTrans.gameObject.SetActive(false);
            }
            tdTimeArray = new float[tdTextCount];
            tdDurationArray = new float[tdTextCount];

            pdColor = pdDmgTextArray[0].color;
            mdColor = mdDmgTextArray[0].color;
            tdColor = tdDmgTextArray[0].color;
        }

        void Update() {
            var dt = Time.deltaTime;
            UpdatePhysicalDamageFloatText(dt);
            UpdateMagicDamageFloatText(dt);
            UpdateTrueDamageFloatText(dt);
        }

        public void ShowDamageFloatText(DamageType damageType, float damageValue, float duration) {
            switch (damageType) {
                case DamageType.Physical:
                    ShowPhysicalDamageFloatText(damageValue, duration);
                    break;
                case DamageType.Magic:
                    ShowMagicDamageFloatText(damageValue, duration);
                    break;
                case DamageType.True:
                    ShowTrueDamageFloatText(damageValue, duration);
                    break;
            }
        }

        void ShowPhysicalDamageFloatText(float damageValue, float duration) {
            var len = pdTextTransArray.Length;
            for (int i = 0; i < len; i++) {
                var textGO = pdTextTransArray[i].gameObject;
                if (!textGO.activeSelf) {
                    textGO.SetActive(true);

                    var pdDmgText = pdDmgTextArray[i];
                    pdDmgText.text = damageValue.ToString();
                    pdDmgText.color = pdColor;

                    textGO.transform.localPosition = GetRandomOffset();
                    textGO.transform.localScale = GetDmgTextScale(damageValue);

                    pdTimeArray[i] = 0;
                    pdDurationArray[i] = duration;
                    break;
                }
            }
        }

        void ShowMagicDamageFloatText(float damageValue, float duration) {
            var len = mdTextTransArray.Length;
            for (int i = 0; i < len; i++) {
                var textGO = mdTextTransArray[i].gameObject;
                if (!textGO.activeSelf) {
                    textGO.SetActive(true);

                    var mdDmgText = mdDmgTextArray[i];
                    mdDmgText.text = damageValue.ToString();
                    mdDmgText.color = mdColor;

                    textGO.transform.localPosition = GetRandomOffset();
                    textGO.transform.localScale = GetDmgTextScale(damageValue);

                    mdTimeArray[i] = 0;
                    mdDurationArray[i] = duration;
                    break;
                }
            }
        }

        void ShowTrueDamageFloatText(float damageValue, float duration) {
            var len = tdTextTransArray.Length;
            for (int i = 0; i < len; i++) {
                var textGO = tdTextTransArray[i].gameObject;
                if (!textGO.activeSelf) {
                    textGO.SetActive(true);

                    var tdDmgText = tdDmgTextArray[i];
                    tdDmgText.text = damageValue.ToString();
                    tdDmgText.color = tdColor;

                    textGO.transform.localPosition = GetRandomOffset();
                    textGO.transform.localScale = GetDmgTextScale(damageValue);

                    tdTimeArray[i] = 0;
                    tdDurationArray[i] = duration;
                    break;
                }
            }
        }

        void UpdatePhysicalDamageFloatText(float dt) {
            var len = pdTextTransArray.Length;
            for (int i = 0; i < len; i++) {
                var go = pdTextTransArray[i].gameObject;
                if (!go.activeSelf) {
                    continue;
                }

                var time = pdTimeArray[i];
                var duration = pdDurationArray[i];
                time += dt;
                if (time >= duration) {
                    go.SetActive(false);
                } else {
                    var pdTextTrans = pdTextTransArray[i];
                    var pdDmgText = pdDmgTextArray[i];
                    pdTextTrans.position += Vector3.up * 0.5f * dt;
                    pdDmgText.color = new Color(pdDmgText.color.r, pdDmgText.color.g, pdDmgText.color.b, 1 - time / duration);
                }
                pdTimeArray[i] = time;
            }
        }

        void UpdateMagicDamageFloatText(float dt) {
            var len = mdTextTransArray.Length;
            for (int i = 0; i < len; i++) {
                var go = mdTextTransArray[i].gameObject;
                if (!go.activeSelf) {
                    continue;
                }

                var time = mdTimeArray[i];
                var duration = mdDurationArray[i];
                time += dt;
                if (time >= duration) {
                    go.SetActive(false);
                } else {
                    var mdTextTrans = mdTextTransArray[i];
                    var mdDmgText = mdDmgTextArray[i];
                    mdTextTrans.position += Vector3.up * 0.5f * dt;
                    mdDmgText.color = new Color(mdDmgText.color.r, mdDmgText.color.g, mdDmgText.color.b, 1 - time / duration);
                }
                mdTimeArray[i] = time;
            }
        }

        void UpdateTrueDamageFloatText(float dt) {
            var len = tdTextTransArray.Length;
            for (int i = 0; i < len; i++) {
                var go = tdTextTransArray[i].gameObject;
                if (!go.activeSelf) {
                    continue;
                }

                var time = tdTimeArray[i];
                var duration = tdDurationArray[i];
                time += dt;
                if (time >= duration) {
                    go.SetActive(false);
                } else {
                    var tdTextTrans = tdTextTransArray[i];
                    var tdDmgText = tdDmgTextArray[i];
                    tdTextTrans.position += Vector3.up * 0.5f * dt;
                    tdDmgText.color = new Color(tdDmgText.color.r, tdDmgText.color.g, tdDmgText.color.b, 1 - time / duration);
                }
                tdTimeArray[i] = time;
            }
        }

        Vector3 GetRandomOffset() {
            var x = Random.Range(-0.5f, 0.5f);
            var y = Random.Range(-0.5f, 0.5f);
            return new Vector3(x, y, 0);
        }

        Vector3 GetDmgTextScale(float damageValue) {
            var scale = 1 + damageValue * 0.005f;
            scale = Mathf.Clamp(scale, 1f, 2f);
            return new Vector3(scale, scale, 1);
        }

    }

}


using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RoleCtrlEffectStateModel {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        RoleCtrlEffectType ctrlEffectType;
        public RoleCtrlEffectType CtrlEffectType => ctrlEffectType;
        public void SetCtrlEffectType(RoleCtrlEffectType value) => ctrlEffectType = value;

        float totalFrame;
        public float TotalFrame => totalFrame;
        public void SetTotalFrame(float value) => totalFrame = value;

        public int curFrame;

        public RoleCtrlEffectStateModel() { }

        public void Reset() {
            isEntering = false;
            totalFrame = 0;
            curFrame = -1;
        }

    }
}
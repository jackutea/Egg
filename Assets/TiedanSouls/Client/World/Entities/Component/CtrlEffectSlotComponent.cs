using System;
using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class CtrlEffectSlotComponent {

        Dictionary<RoleCtrlEffectType, List<RoleCtrlEffectStateModel>> ctrlEffectListDict;

        public CtrlEffectSlotComponent() {
            ctrlEffectListDict = new Dictionary<RoleCtrlEffectType, List<RoleCtrlEffectStateModel>>();
            ctrlEffectListDict.Add(RoleCtrlEffectType.Root, new List<RoleCtrlEffectStateModel>());
            ctrlEffectListDict.Add(RoleCtrlEffectType.Stun, new List<RoleCtrlEffectStateModel>());
            ctrlEffectListDict.Add(RoleCtrlEffectType.Silence, new List<RoleCtrlEffectStateModel>());
        }

        public void Reset() {
            ctrlEffectListDict[RoleCtrlEffectType.Root].Clear();
            ctrlEffectListDict[RoleCtrlEffectType.Stun].Clear();
            ctrlEffectListDict[RoleCtrlEffectType.Silence].Clear();
        }

        public void ResetRootCtrlEffect() {
            ctrlEffectListDict[RoleCtrlEffectType.Root].Clear();
        }

        public void ResetStunCtrlEffect() {
            ctrlEffectListDict[RoleCtrlEffectType.Stun].Clear();
        }

        public void ResetSilenceCtrlEffect() {
            ctrlEffectListDict[RoleCtrlEffectType.Silence].Clear();
        }

        public void AddCtrlEffect(RoleCtrlEffectStateModel ctrlEffectModel) {
            ctrlEffectListDict[ctrlEffectModel.CtrlEffectType].Add(ctrlEffectModel);
        }

    }

}
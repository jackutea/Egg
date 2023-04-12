using System;
using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleCtrlEffectSlotComponent {

        Dictionary<RoleCtrlEffectType, List<RoleCtrlEffect>> ctrlEffectListDict;

        public RoleCtrlEffectSlotComponent() {
            ctrlEffectListDict = new Dictionary<RoleCtrlEffectType, List<RoleCtrlEffect>>();
            ctrlEffectListDict.Add(RoleCtrlEffectType.Root, new List<RoleCtrlEffect>());
            ctrlEffectListDict.Add(RoleCtrlEffectType.Stun, new List<RoleCtrlEffect>());
            ctrlEffectListDict.Add(RoleCtrlEffectType.Silence, new List<RoleCtrlEffect>());
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

        public void AddCtrlEffect(in RoleCtrlEffect ctrlEffectModel) {
            var father = ctrlEffectModel.father;
            var list = ctrlEffectListDict[ctrlEffectModel.ctrlEffectType];
            for (int i = 0; i < list.Count; i++) {
                var effect = list[i];
                if (effect.father.IsTheSameAs(father)) {
                    list[i] = ctrlEffectModel;
                    TDLog.Log($"刷新 技能[{father.typeID}] 控制效果 --》 {ctrlEffectModel}");
                    return;
                }
            }

            TDLog.Log($"添加 技能[{father.typeID}] 控制效果 --》 {ctrlEffectModel}");
            list.Add(ctrlEffectModel);
        }

    }

}
using System;
using System.Collections.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillorSlotComponent {

        Dictionary<int, SkillorModel> originalSkillors;

        Dictionary<int, SkillorModel> comboSkillors;

        Dictionary<SkillorType, SkillorModel> all_sortedByType;

        public SkillorSlotComponent() {
            originalSkillors = new Dictionary<int, SkillorModel>();
            comboSkillors = new Dictionary<int, SkillorModel>();
            all_sortedByType = new Dictionary<SkillorType, SkillorModel>();
        }

        public void Reset() {
            var e = originalSkillors.Values.GetEnumerator();
            while (e.MoveNext()) {
                e.Current.Reset();
            }
        }

        public void AddOriginalSkillor(SkillorModel model) {
            if (model.SkillorType == SkillorType.None) {
                TDLog.Warning($"原始技能: {model.TypeID} 技能类型: {SkillorType.None}");
                return;
            }

            originalSkillors.Add(model.TypeID, model);
            all_sortedByType.TryAdd(model.SkillorType, model);
            TDLog.Log($"加载原始技能: {model.TypeID}");
        }

        public void AddComboSkillor(SkillorModel model) {
            if (model.SkillorType == SkillorType.None) {
                TDLog.Warning($"连击技能: {model.TypeID} 技能类型: {SkillorType.None}");
                return;
            }

            comboSkillors.Add(model.TypeID, model);
            TDLog.Log($"加载连击技能: {model.TypeID}");
        }

        public void Remove(int typeID) {
            if (!originalSkillors.TryGetValue(typeID, out var model)) {
                return;
            }

            originalSkillors.Remove(typeID);
            all_sortedByType.Remove(model.SkillorType);
        }

        public bool TryGetOriginalSkillor(int typeID, out SkillorModel model) {
            return originalSkillors.TryGetValue(typeID, out model);
        }

        public bool TryGetOriginalSkillorByType(SkillorType type, out SkillorModel model) {
            return all_sortedByType.TryGetValue(type, out model);
        }

        public bool TryGetComboSkillor(int typeID, out SkillorModel model) {
            return comboSkillors.TryGetValue(typeID, out model);
        }

        public bool TryGetComboSkillorByType(SkillorType type, out SkillorModel model) {
            return all_sortedByType.TryGetValue(type, out model);
        }

        public bool HasOriginalSkillor(int typeID) {
            return originalSkillors.ContainsKey(typeID);
        }

        public bool HasComboSkillor(int typeID) {
            return comboSkillors.ContainsKey(typeID);
        }

        public void ForeachAllOriginalSkillor(Action<SkillorModel> action) {
            var e = originalSkillors.Values.GetEnumerator();
            while (e.MoveNext()) {
                action(e.Current);
            }
        }

        public void ForeachAllComboSkillor(Action<SkillorModel> action) {
            var e = comboSkillors.Values.GetEnumerator();
            while (e.MoveNext()) {
                action(e.Current);
            }
        }

    }

}
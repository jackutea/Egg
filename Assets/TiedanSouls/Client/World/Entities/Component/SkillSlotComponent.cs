using System;
using System.Collections.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillSlotComponent {

        Dictionary<int, SkillModel> originalSkills;

        Dictionary<int, SkillModel> comboSkills;

        Dictionary<SkillType, SkillModel> all_sortedByType;

        public SkillSlotComponent() {
            originalSkills = new Dictionary<int, SkillModel>();
            comboSkills = new Dictionary<int, SkillModel>();
            all_sortedByType = new Dictionary<SkillType, SkillModel>();
        }

        public void Reset() {
            var e = originalSkills.Values.GetEnumerator();
            while (e.MoveNext()) {
                e.Current.Reset();
            }
        }

        public void AddOriginalSkill(SkillModel model) {
            if (model.SkillType == SkillType.None) {
                TDLog.Warning($"原始技能: {model.TypeID} 技能类型: {SkillType.None}");
                return;
            }

            originalSkills.Add(model.TypeID, model);
            all_sortedByType.TryAdd(model.SkillType, model);
            TDLog.Log($"加载原始技能: {model.TypeID}");
        }

        public void AddComboSkill(SkillModel model) {
            if (model.SkillType == SkillType.None) {
                TDLog.Warning($"连击技能: {model.TypeID} 技能类型: {SkillType.None}");
                return;
            }

            comboSkills.Add(model.TypeID, model);
            TDLog.Log($"加载连击技能: {model.TypeID}");
        }

        public void Remove(int typeID) {
            if (!originalSkills.TryGetValue(typeID, out var model)) {
                return;
            }

            originalSkills.Remove(typeID);
            all_sortedByType.Remove(model.SkillType);
        }

        public bool TryGetOriginalSkillByTypeID(int typeID, out SkillModel model) {
            return originalSkills.TryGetValue(typeID, out model);
        }

        public bool TryGetOriginalSkillByType(SkillType type, out SkillModel model) {
            return all_sortedByType.TryGetValue(type, out model);
        }

        public bool TryGetComboSkill(int typeID, out SkillModel model) {
            return comboSkills.TryGetValue(typeID, out model);
        }

        public bool TryGetComboSkillByType(SkillType type, out SkillModel model) {
            return all_sortedByType.TryGetValue(type, out model);
        }

        public bool HasOriginalSkill(int typeID) {
            return originalSkills.ContainsKey(typeID);
        }

        public bool HasComboSkill(int typeID) {
            return comboSkills.ContainsKey(typeID);
        }

        public void ForeachAllOriginalSkill(Action<SkillModel> action) {
            var e = originalSkills.Values.GetEnumerator();
            while (e.MoveNext()) {
                action(e.Current);
            }
        }

        public void ForeachAllComboSkill(Action<SkillModel> action) {
            var e = comboSkills.Values.GetEnumerator();
            while (e.MoveNext()) {
                action(e.Current);
            }
        }

    }

}
using System.Collections.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillorSlotComponent {

        Dictionary<int, SkillorModel> originalSkillors;

        Dictionary<int, SkillorModel> comboSkillors;

        Dictionary<SkillorType, SkillorModel> allByType;

        public SkillorSlotComponent() {
            originalSkillors = new Dictionary<int, SkillorModel>();
            comboSkillors = new Dictionary<int, SkillorModel>();
            allByType = new Dictionary<SkillorType, SkillorModel>();
        }

        public void AddOriginalSkillor(SkillorModel model) {
            if (model.SkillorType == SkillorType.None) {
                TDLog.Warning($"原始技能: {model.TypeID} 技能类型: {SkillorType.None}");
                return;
            }

            originalSkillors.Add(model.TypeID, model);
            allByType.TryAdd(model.SkillorType, model);
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
            allByType.Remove(model.SkillorType);
        }

        public bool TryGetOriginalSkillor(int typeID, out SkillorModel model) {
            return originalSkillors.TryGetValue(typeID, out model);
        }

        public bool TryGetOriginalSkillorByType(SkillorType type, out SkillorModel model) {
            return allByType.TryGetValue(type, out model);
        }

        public bool TryGetComboSkillor(int typeID, out SkillorModel model) {
            return comboSkillors.TryGetValue(typeID, out model);
        }

        public bool TryGetComboSkillorByType(SkillorType type, out SkillorModel model) {
            return allByType.TryGetValue(type, out model);
        }

        public bool HasOriginalSkillor(int typeID) {
            return originalSkillors.ContainsKey(typeID);
        }

        public bool HasComboSkillor(int typeID) {
            return comboSkillors.ContainsKey(typeID);
        }

        public IEnumerable<SkillorModel> GetAll() {
            return originalSkillors.Values;
        }

    }

}
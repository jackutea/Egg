using System;
using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class SkillSlotComponent {

        // 原始技能名单 ( Key = TypeID )
        Dictionary<int, SkillEntity> skillDic_origin;

        // 组合技技能名单 ( Key = TypeID )
        Dictionary<int, SkillEntity> skillDic_combo;

        public SkillSlotComponent() {
            skillDic_origin = new Dictionary<int, SkillEntity>();
            skillDic_combo = new Dictionary<int, SkillEntity>();
        }

        #region [增]

        public bool TryAdd_Origin(in SkillEntity skillEntity) {
            var idCom = skillEntity.IDCom;
            var tid = idCom.TypeID;
            if (skillDic_origin.ContainsKey(tid)) {
                return false;
            }
            skillDic_origin.Add(tid, skillEntity);
            return true;
        }

        public bool TryAdd_Combo(in SkillEntity skillEntity) {
            var idCom = skillEntity.IDCom;
            var tid = idCom.TypeID;
            if (skillDic_combo.ContainsKey(tid)) {
                return false;
            }
            skillDic_combo.Add(tid, skillEntity);
            return true;
        }

        #endregion

        #region [删]

        public bool TryRemove_Origin(in SkillEntity skillEntity) {
            var idCom = skillEntity.IDCom;
            var tid = idCom.TypeID;
            if (!skillDic_origin.ContainsKey(tid)) {
                return false;
            }
            skillDic_origin.Remove(tid);
            return true;
        }

        public bool TryRemove_Combo(in SkillEntity skillEntity) {
            var idCom = skillEntity.IDCom;
            var tid = idCom.TypeID;
            if (!skillDic_combo.ContainsKey(tid)) {
                return false;
            }
            skillDic_combo.Remove(tid);
            return true;
        }

        #endregion

        #region [改]

        public void SetFather(EntityIDComponent father) {
            var e = skillDic_origin.Values.GetEnumerator(); 
            while (e.MoveNext()) {
                var skill = e.Current;
                skill.SetFather(father);
            }
            e = skillDic_combo.Values.GetEnumerator();
            while (e.MoveNext()) {
                var skill = e.Current;
                skill.SetFather(father);
            }
        }

        #endregion

        #region [查]

        public bool TryGet(int skillTypeID, out SkillEntity skillEntity) {
            if (skillDic_origin.TryGetValue(skillTypeID, out skillEntity)) return true;
            if (skillDic_combo.TryGetValue(skillTypeID, out skillEntity)) return true;
            return false;
        }

        public bool TryGet(int skillTypeID, bool isCombo, out SkillEntity skillEntity) {
            if (skillDic_origin.TryGetValue(skillTypeID, out skillEntity)) {
                return true;
            }
            if (skillDic_combo.TryGetValue(skillTypeID, out skillEntity)) {
                return true;
            }
            return false;
        }

        public bool TryGet_Origin(int tid, out SkillEntity skillEntity) {
            return skillDic_origin.TryGetValue(tid, out skillEntity);
        }

        public bool TryGet_Combo(int tid, out SkillEntity skillEntity) {
            return skillDic_combo.TryGetValue(tid, out skillEntity);
        }

        public bool TrgGet_OriginSkill_BySkillType(SkillType skillType, out SkillEntity skillEntity) {
            var e = skillDic_origin.Values.GetEnumerator();
            while (e.MoveNext()) {
                var skill = e.Current;
                if (skill.SkillType == skillType) {
                    skillEntity = skill;
                    return true;
                }
            }
            skillEntity = null;
            return false;
        }

        public void Foreach_Origin(Action<SkillEntity> action) {
            foreach (var skill in skillDic_origin.Values) {
                action(skill);
            }
        }

        public void Foreach_Combo(Action<SkillEntity> action) {
            foreach (var skill in skillDic_combo.Values) {
                action(skill);
            }
        }

        #endregion

    }

}
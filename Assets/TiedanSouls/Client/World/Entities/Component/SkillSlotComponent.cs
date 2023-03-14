using System;
using System.Collections.Generic;

namespace TiedanSouls.World.Entities {

    public class SkillSlotComponent {

        // 原始技能名单 ( Key = TypeID )
        Dictionary<int, SkillEntity> skillDic_origin;

        // 连招技能名单 ( Key = TypeID )
        Dictionary<int, SkillEntity> skillDic_combo;

        // 可强制取消

        public SkillSlotComponent() {
            skillDic_origin = new Dictionary<int, SkillEntity>();
            skillDic_combo = new Dictionary<int, SkillEntity>();
        }

        #region [增加]

        public bool TryAdd_Origin(in SkillEntity skillSkillEntity) {
            var idCom = skillSkillEntity.IDCom;
            var tid = idCom.TypeID;
            if (skillDic_origin.ContainsKey(tid)) {
                return false;
            }
            skillDic_origin.Add(tid, skillSkillEntity);
            return true;
        }

        public bool TryAdd_Combo(in SkillEntity skillSkillEntity) {
            var idCom = skillSkillEntity.IDCom;
            var tid = idCom.TypeID;
            if (skillDic_combo.ContainsKey(tid)) {
                return false;
            }
            skillDic_combo.Add(tid, skillSkillEntity);
            return true;
        }

        #endregion

        #region [删除]

        public bool TryRemove_Origin(in SkillEntity skillSkillEntity) {
            var idCom = skillSkillEntity.IDCom;
            var tid = idCom.TypeID;
            if (!skillDic_origin.ContainsKey(tid)) {
                return false;
            }
            skillDic_origin.Remove(tid);
            return true;
        }

        public bool TryRemove_Combo(in SkillEntity skillSkillEntity) {
            var idCom = skillSkillEntity.IDCom;
            var tid = idCom.TypeID;
            if (!skillDic_combo.ContainsKey(tid)) {
                return false;
            }
            skillDic_combo.Remove(tid);
            return true;
        }

        #endregion

        #region [查询]

        public bool TryGet_Origin(int tid, out SkillEntity skillSkillEntity) {
            return skillDic_origin.TryGetValue(tid, out skillSkillEntity);
        }

        public bool TryGet_Combo(int tid, out SkillEntity skillSkillEntity) {
            return skillDic_combo.TryGetValue(tid, out skillSkillEntity);
        }

        public bool TrgGetOriginSkill_BySkillType(SkillType skillType, out SkillEntity skillSkillEntity) {
            var e = skillDic_origin.Values.GetEnumerator();
            while (e.MoveNext()) {
                var skill = e.Current;
                if (skill.SkillType == skillType) {
                    skillSkillEntity = skill;
                    return true;
                }
            }
            skillSkillEntity = null;
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
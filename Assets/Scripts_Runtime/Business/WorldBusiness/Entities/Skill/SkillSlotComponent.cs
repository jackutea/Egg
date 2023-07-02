using System;
using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class SkillSlotComponent {

        // 原始技能名单 ( Key = TypeID )
        Dictionary<int, SkillEntity> all;

        public SkillSlotComponent() {
            all = new Dictionary<int, SkillEntity>();
        }

        #region [增]
        public bool TryAdd(in SkillEntity skillEntity) {
            var idCom = skillEntity.IDCom;
            var tid = idCom.TypeID;
            if (all.ContainsKey(tid)) {
                return false;
            }
            all.Add(tid, skillEntity);
            return true;
        }
        #endregion

        #region [删]
        public bool TryRemove(in SkillEntity skillEntity) {
            var idCom = skillEntity.IDCom;
            var tid = idCom.TypeID;
            if (!all.ContainsKey(tid)) {
                return false;
            }
            all.Remove(tid);
            return true;
        }
        #endregion

        #region [改]

        public void SetFather(EntityIDComponent father) {
            var e = all.Values.GetEnumerator(); 
            while (e.MoveNext()) {
                var skill = e.Current;
                skill.SetFather(father);
            }
        }
        #endregion

        #region [查]
        public bool TryGet(int tid, out SkillEntity skillEntity) {
            return all.TryGetValue(tid, out skillEntity);
        }

        public bool TrgGetByCastKey(SkillCastKey castKey, out SkillEntity skillEntity) {
            foreach (var skill in all.Values) {
                if (skill.CastKey == castKey) {
                    skillEntity = skill;
                    return true;
                }
            }
            skillEntity = null;
            return false;
        }

        public void Foreach(Action<SkillEntity> action) {
            foreach (var skill in all.Values) {
                action(skill);
            }
        }

        #endregion

    }

}
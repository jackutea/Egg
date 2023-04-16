using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public class SkillRepo {

        Dictionary<int, SkillEntity> allSkills;

        public SkillRepo() {
            allSkills = new Dictionary<int, SkillEntity>();
        }

        public bool TryAdd(SkillEntity skill) {
            return allSkills.TryAdd(skill.IDCom.EntityID, skill);
        }

        public bool TryGet(int entityID, out SkillEntity skill) {
            return allSkills.TryGetValue(entityID, out skill);
        }

    }
}
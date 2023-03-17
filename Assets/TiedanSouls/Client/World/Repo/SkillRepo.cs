using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public class SkillRepo {

        Dictionary<int, SkillEntity> allSkills;

        public SkillRepo() {
            allSkills = new Dictionary<int, SkillEntity>();
        }

        public bool TryGet(int entityID, out SkillEntity skill) {
            skill = null;
            return allSkills.TryGetValue(entityID, out skill);
        }

    }
}
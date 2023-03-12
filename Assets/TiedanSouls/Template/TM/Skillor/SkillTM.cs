using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillTM {

        // - Identity
        public int typeID;
        public string name;
        public SkillType skillType;

        // - Combo
        public int originalSkillTypeID;

        // - Renderer
        public string weaponAnimName;

        // - Hit Power
        public HitPowerTM[] hitPowerArray;

        public SkillTM() { }

    }

}
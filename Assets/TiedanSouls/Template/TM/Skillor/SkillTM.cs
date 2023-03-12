using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillTM {

        // - Identity
        public int typeID;
        public string skillName;
        public SkillType skillType;

        // - Combo
        public int originalSkillTypeID;

        // - Renderer
        public string weaponAnimName;

        // - Hit Power
        public HitPowerTM[] hitPowerArray;

        // - Collision Trigger
        public CollisionTriggerTM[] collisionTriggerTMArray;

        public SkillTM() { }

    }

}
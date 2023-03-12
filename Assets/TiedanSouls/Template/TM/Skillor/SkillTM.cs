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

        // - Frame
        public SkillFrameTM[] frames;

        // - Renderer
        public string weaponAnimName;
        
         public SkillTM() { }

    }

}
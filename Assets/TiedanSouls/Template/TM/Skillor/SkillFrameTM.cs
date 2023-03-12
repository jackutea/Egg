using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillFrameTM {

        // - Hit
        public HitPowerTM hitPower;

        // - Dash
        public bool hasDash;
        public Vector2 dashForce;
        public bool isDashEnd;

        // - Cancel
        public SkillCancelTM[] cancelTMs;

        public SkillBoxTM[] boxes;

        public SkillFrameTM() { }

    }

}
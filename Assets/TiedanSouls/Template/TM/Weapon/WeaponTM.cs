using System;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public class WeaponTM {

        public WeaponType weaponType;
        public int typeID;

        public int atk;
        public int def;
        public int crit;

        public int skillMeleeTypeID;
        public int skillHoldMeleeTypeID;
        public int skillSpecMeleeTypeID;

        // Renderer
        public string meshName;

    }

}
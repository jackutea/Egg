using System;

namespace TiedanSouls.Template {

    [Serializable]
    public class WeaponTM {

        public WeaponType weaponType;
        public int typeID;

        public int atk;
        public int def;
        public int crit;

        public int skillorMeleeTypeID;
        public int skillorHoldMeleeTypeID;
        public int skillorSpecMeleeTypeID;

        // Renderer
        public string meshName;

    }

}
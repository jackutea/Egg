using System;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    /// <summary>
    /// ComparisonType 为 None 时，不进行对比,即不在条件中
    /// </summary>
    [Serializable]
    public struct RoleSelectorEM {

        public int hp;
        public ComparisonType hp_ComparisonType;

        public int hpMax;
        public ComparisonType hpMax_ComparisonType;

        public int mp;
        public ComparisonType mp_ComparisonType;

        public int mpMax;
        public ComparisonType mpMax_ComparisonType;

        public int gp;
        public ComparisonType gp_ComparisonType;

        public int gpMax;
        public ComparisonType gpMax_ComparisonType;

    }

}
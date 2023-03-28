using System;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 属性影响模型
    /// </summary>
    [Serializable]
    public struct AttributeEffectModel {

        public NumCalculationType hpNCT;
        public int hpEV;

        public NumCalculationType hpMaxNCT;
        public int hpMaxEV;

        public NumCalculationType atkPowerNCT;
        public int atkPowerEV;

        public NumCalculationType atkSpeedNCT;
        public int atkSpeedEV;

        public NumCalculationType moveSpeedNCT;
        public int moveSpeedEV;

    }

}
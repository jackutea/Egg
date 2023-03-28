using System;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 属性影响模型
    /// </summary>
    [Serializable]
    public struct AttributeEffectModel {

        public NumCalculationType hpNCT;
        public float hpEV;

        public NumCalculationType maxHPNCT;
        public float maxHPEV;

        public NumCalculationType atkPowerNCT;
        public float atkPowerEV;

        public NumCalculationType atkSpeedNCT;
        public float atkSpeedEV;

        public NumCalculationType moveSpeedNCT;
        public float moveSpeedEV;

    }

}
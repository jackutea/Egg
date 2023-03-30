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
        public bool needRevokeHPEV;

        public NumCalculationType hpMaxNCT;
        public int hpMaxEV;
        public bool needRevokeHPMaxEV;

        public NumCalculationType atkPowerNCT;
        public int atkPowerEV;
        public bool needRevokeAtkPowerEV;

        public NumCalculationType atkSpeedNCT;
        public int atkSpeedEV;
        public bool needRevokeAtkSpeedEV;

        public NumCalculationType moveSpeedNCT;
        public int moveSpeedEV;
        public bool needRevokeMoveSpeedEV;

    }

}
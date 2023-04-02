using System;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 属性影响模型
    /// </summary>
    [Serializable]
    public struct RoleAttributeEffectModel {

        public NumCalculationType hpNCT;
        public int hpEV;
        public int hpEffectTimes;
        public bool needRevoke_HPEV;

        public NumCalculationType hpMaxNCT;
        public int hpMaxEV;
        public int hpMaxEffectTimes;
        public bool needRevoke_HPMaxEV;

        public int physicsDamageEV;
        public int physicsDamageEffectTimes;
        public bool needRevoke_PhysicsDamageEV;

        public int magicDamageEV;
        public int magicDamageEffectTimes;
        public bool needRevoke_MagicPowerEV;

        public NumCalculationType atkSpeedNCT;
        public int atkSpeedEV;
        public int atkSpeedEffectTimes;
        public bool needRevoke_AtkSpeedEV;

        public NumCalculationType moveSpeedNCT;
        public int moveSpeedEV;
        public int moveSpeedEffectTimes;
        public bool needRevoke_MoveSpeedEV;

    }

}
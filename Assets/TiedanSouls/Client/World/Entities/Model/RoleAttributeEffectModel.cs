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
        public int hpEffectTimes;
        public bool needRevoke_HPEV;

        public NumCalculationType hpMaxNCT;
        public int hpMaxEV;
        public int hpMaxEffectTimes;
        public bool needRevoke_HPMaxEV;

        public NumCalculationType moveSpeedNCT;
        public int moveSpeedEV;
        public int moveSpeedEffectTimes;
        public bool needRevoke_MoveSpeedEV;

        public int physicsDamageBonusEV;
        public int physicsDamageBonusEffectTimes;
        public bool needRevokePhysicsDamageBonusEV;

        public int magicDamageBonusEV;
        public int magicDamageBonusEffectTimes;
        public bool needRevokeMagicDamageBonus;

        public int physicsDefenseBonusEV;
        public int physicsDefenseBonusEffectTimes;
        public bool needRevokePhysicsDefenseBonus;

        public int magicDefenseBonusEV;
        public int magicDefenseBonusEffectTimes;
        public bool needRevokeMagicDefenseBonus;

    }

}
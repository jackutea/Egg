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
        public float hpEffectTimes;
        public bool needRevoke_HPEV;
        public float hpOffset;

        public NumCalculationType hpMaxNCT;
        public float hpMaxEV;
        public float hpMaxEffectTimes;
        public bool needRevoke_HPMaxEV;
        public float hpMaxOffset;

        public NumCalculationType moveSpeedNCT;
        public float moveSpeedEV;
        public float moveSpeedEffectTimes;
        public bool needRevoke_MoveSpeedEV;
        public float moveSpeedOffset;

        public float physicalDamageBonusEV;
        public float physicalDamageBonusEffectTimes;
        public bool needRevokePhysicalDamageBonusEV;
        public float physicalDamageBonusOffset;

        public float magicalDamageBonusEV;
        public float magicalDamageBonusEffectTimes;
        public bool needRevokemagicalDamageBonus;
        public float magicalDamageBonusOffset;

        public float physicalDefenseBonusEV;
        public float physicalDefenseBonusEffectTimes;
        public bool needRevokePhysicalDefenseBonus;
        public float physicalDefenseBonusOffset;

        public float magicalDefenseBonusEV;
        public float magicalDefenseBonusEffectTimes;
        public bool needRevokemagicalDefenseBonus;
        public float magicalDefenseBonusOffset;

    }

}
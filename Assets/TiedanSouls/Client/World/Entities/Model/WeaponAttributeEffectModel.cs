using System;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 武器属性影响模型
    /// </summary>
    [Serializable]
    public struct WeaponAttributeEffectModel {

        public int physicsDamageIncreaseEV;
        public int physicsDamageIncreaseEffectTimes;
        public bool needRevokePhysicsDamageIncreaseEV;

        public int magicDamageIncreaseEV;
        public int magicDamageIncreaseEffectTimes;
        public bool needRevokeMagicDamageIncrease;

    }

}
using System;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 属性影响模型
    /// </summary>
    [Serializable]
    public class AttributeEffectModel {

        public NumCalculationType hpNCT;
        public float hpEV;
        public float hpEffectTimes;
        public float hpOffset;
        public bool hpNeedRevoke;

        public NumCalculationType hpMaxNCT;
        public float hpMaxEV;
        public float hpMaxEffectTimes;
        public float hpMaxOffset;
        public bool hpMaxNeedRevoke;

        public NumCalculationType moveSpeedNCT;
        public float moveSpeedEV;
        public float moveSpeedEffectTimes;
        public float moveSpeedOffset;
        public bool moveSpeedRevoke;

        public float normalSkillSpeedBonusEV;
        public float normalSkillSpeedBonusEffectTimes;
        public float normalSkillSpeedBonusOffset;
        public bool normalSkillSpeedBonusNeedRevoke;

        public float physicalDamageBonusEV;
        public float physicalDamageBonusEffectTimes;
        public float physicalDamageBonusOffset;
        public bool physicalDamageBonusNeedRevoke;

        public float magicalDamageBonusEV;
        public float magicalDamageBonusEffectTimes;
        public float magicalDamageBonusOffset;
        public bool magicalDamageBonusNeedRevoke;

        public float physicalDefenseBonusEV;
        public float physicalDefenseBonusEffectTimes;
        public float physicalDefenseBonusOffset;
        public bool needRevokePhysicalDefenseBonus;

        public float magicalDefenseBonusEV;
        public float magicalDefenseBonusEffectTimes;
        public float magicalDefenseBonusOffset;
        public bool magicalDefenseBonusNeedRevoke;

        public void ResetOffset() {
            hpOffset = 0;
            hpMaxOffset = 0;
            moveSpeedOffset = 0;
            normalSkillSpeedBonusOffset = 0;
            physicalDamageBonusOffset = 0;
            magicalDamageBonusOffset = 0;
            physicalDefenseBonusOffset = 0;
            magicalDefenseBonusOffset = 0;
        }

    }

}
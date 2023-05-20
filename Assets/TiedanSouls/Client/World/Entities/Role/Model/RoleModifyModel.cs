using System;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    [Serializable]
    public class RoleModifyModel {

        public NumCalculationType hpNCT;
        public float hpEV;
        public float hpOffset;

        public NumCalculationType hpMaxNCT;
        public float hpMaxEV;
        public float hpMaxOffset;

        public NumCalculationType moveSpeedNCT;
        public float moveSpeedEV;
        public float moveSpeedOffset;

        public float normalSkillSpeedBonusEV;
        public float normalSkillSpeedBonusOffset;

        public float physicalDamageBonusEV;
        public float physicalDamageBonusOffset;

        public float magicalDamageBonusEV;
        public float magicalDamageBonusOffset;

        public float physicalDefenseBonusEV;
        public float physicalDefenseBonusOffset;

        public float magicalDefenseBonusEV;
        public float magicalDefenseBonusOffset;

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
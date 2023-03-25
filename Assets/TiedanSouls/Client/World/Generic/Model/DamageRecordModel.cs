using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    /// <summary>
    /// 伤害记录模型
    /// </summary>
    public struct DamageRecordModel {

        public IDArgs attacker;         // 攻击者
        public IDArgs victim;           // 受害者

        public DamageType damageType;   // 伤害类型
        public int damage;              // 伤害值

    }
}
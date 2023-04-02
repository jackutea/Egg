using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 伤害结算服务
    /// </summary>
    public class DamageArbitService {

        Dictionary<ulong, List<DamageRecordModel>> all;

        public DamageArbitService() {
            all = new Dictionary<ulong, List<DamageRecordModel>>();
        }

        /// <summary>
        /// 仲裁伤害
        /// </summary>
        public float ArbitrateDamage(DamageType damageType,
                                    int damage,
                                    AttributeComponent atkerAttributeCom,
                                    AttributeComponent victimAttributeCom) {
            if (damageType == DamageType.True) {
                return damage;
            }

            // 伤害 加成 & 减免
            if (damageType == DamageType.Physical) {
                var physicalDamageBonus = atkerAttributeCom.PhysicalDamageBonus;
                var physicDamage = damage * (1 + physicalDamageBonus);
                var physicalDefenseBonus = victimAttributeCom.PhysicalDefenseBonus;
                var realDamage = physicDamage * (1 - physicalDefenseBonus);
                return realDamage;
            }

            if (damageType == DamageType.Magic) {
                var magicalDamageBonus = atkerAttributeCom.MagicalDamageBonus;
                var magicalDamage = damage * (1 + magicalDamageBonus);
                var magicalDefenseBonus = victimAttributeCom.MagicalDefenseBonus;
                var realDamage = magicalDamage * (1 - magicalDefenseBonus);
                return realDamage;
            }

            TDLog.Error("伤害仲裁服务 - 未知伤害类型");
            return 0;
        }

        /// <summary>
        /// 添加伤害记录
        /// </summary>s
        public void Add(DamageType damageType, float damage, in EntityIDArgs victim, in EntityIDArgs attacker) {
            var key = GetKey(attacker, victim);
            if (!all.TryGetValue(key, out var list)) {
                list = new List<DamageRecordModel>();
                all.Add(key, list);
            }

            var record = new DamageRecordModel();
            record.damageType = damageType;
            record.damage = damage;
            record.victim = victim;
            record.attacker = attacker;
            list.Add(record);
            TDLog.Log($"伤害仲裁服务 - 添加伤害记录\n{record}");
        }

        /// <summary>
        /// 获取键值. 保证key1 > key2
        /// </summary>
        public ulong GetKey(in EntityIDArgs attacker, in EntityIDArgs victim) {
            var key1 = ComineToKey(attacker.entityType, attacker.entityID);
            var key2 = ComineToKey(victim.entityType, victim.entityID);
            if (key1 < key2) {
                key1 = key1 ^ key2;
                key2 = key1 ^ key2;
                key1 = key1 ^ key2;
            }
            return key1 << 32 | key2;
        }

        ulong ComineToKey(EntityType entityType, int entityID) => (ulong)entityType << 32 | (uint)entityID;

    }

}
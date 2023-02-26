using System.Collections.Generic;

namespace TiedanSouls.World.Entities {

    public class DamageArbitService {

        Dictionary<ulong, List<ulong>> all;

        public DamageArbitService() {
            all = new Dictionary<ulong, List<ulong>>();
        }

        public void TryAdd(EntityType casterEntityType, int casterID, EntityType victimEntityType, int victimID) {
            ulong casterKey = (ulong)casterEntityType << 32 | (uint)casterID;
            ulong victimKey = (ulong)victimEntityType << 32 | (uint)victimID;
            if (all.ContainsKey(casterKey)) {
                if (!all[casterKey].Contains(victimKey)) {
                    all[casterKey].Add(victimKey);
                }
            } else {
                all.Add(casterKey, new List<ulong>() { victimKey });
            }
            TDLog.Log("Add DamageArbitService: " + casterKey);
        }

        public bool IsInArbit(EntityType casterEntityType, int casterID, EntityType victimEntityType, int victimID) {
            ulong casterKey = (ulong)casterEntityType << 32 | (uint)casterID;
            ulong victimKey = (ulong)victimEntityType << 32 | (uint)victimID;
            if (all.ContainsKey(casterKey)) {
                return all[casterKey].Contains(victimKey);
            }
            return false;
        }

        public void Remove(EntityType casterEntityType, int casterID) {
            ulong casterKey = (ulong)casterEntityType << 32 | (uint)casterID;
            all.Remove(casterKey);
        }

    }

}
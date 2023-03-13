using System.Collections.Generic;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World {

    public class SkillRepo {

        Dictionary<ulong, SkillEntity> skillDic;

        public SkillRepo() {
            skillDic = new Dictionary<ulong, SkillEntity>();
        }

        public bool TryAdd(SkillEntity entity) {
            var key = GetKeyByIDCom(entity.IDCom);
            if (skillDic.ContainsKey(key)) {
                return false;
            }
            skillDic.Add(key, entity);
            return true;
        }

        public bool TryGet(int typeID, int entityID, out SkillEntity entity) {
            var key = CombineToKey(typeID, entityID);
            return skillDic.TryGetValue(key, out entity);
        }

        //  [TypeID] - [EntityID]
        ulong GetKeyByIDCom(IDComponent idCom) {
            var tid = idCom.TypeID;
            var eid = idCom.EntityID;
            return CombineToKey(tid, eid);
        }

        ulong CombineToKey(int typeID, int entityID) {
            var key = (ulong)entityID;
            key |= (ulong)typeID << 32;
            return key;
        }

    }

}
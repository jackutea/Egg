using GameArki.BTTreeNS;
using TiedanSouls.Client.Facades;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RolePatrolAIPrecondition : IBTTreePrecondition {

        RoleEntity role;
        WorldContext worldContext;
        float sight;
        
        public RolePatrolAIPrecondition():this(10) { }

        public RolePatrolAIPrecondition(float sight) {
            this.sight = sight;
        }

        public void Inject(RoleEntity role, WorldContext worldContext) {
            this.role = role;
            this.worldContext = worldContext;
        }

        bool IBTTreePrecondition.CanEnter() {
            Vector2 pos_role = role.RootPos;
            Vector2 pos_target = worldContext.RoleRepo.PlayerRole.RootPos;
            TDLog.Log("检测能否进入");
            return (Vector2.Distance(pos_role, pos_target) > sight);
        }

    }

}
using GameArki.BTTreeNS;
using TiedanSouls.World.Facades;
using UnityEngine;

namespace TiedanSouls.World.Entities {

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
            Vector2 pos_role = role.GetPos_RendererRoot();
            Vector2 pos_target = worldContext.RoleRepo.PlayerRole.GetPos_RendererRoot();
            TDLog.Log("检测能否进入");
            return (Vector2.Distance(pos_role, pos_target) > sight);
        }

    }

}
using GameArki.BTTreeNS;
using TiedanSouls.Client.Facades;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

public class RoleFollowAIPrecondition  : IBTTreePrecondition {

        RoleEntity role;
        WorldContext worldContext;
        float attackRange;
        
        public RoleFollowAIPrecondition():this(1) { }

        public RoleFollowAIPrecondition(float attackRange) {
            this.attackRange = attackRange;
        }

        public void Inject(RoleEntity role, WorldContext worldContext) {
            this.role = role;
            this.worldContext = worldContext;
        }

        bool IBTTreePrecondition.CanEnter() {
            Vector2 pos_role = role.RendererPos;
            Vector2 pos_target = worldContext.RoleRepo.PlayerRole.RendererPos;
            return (Vector2.Distance(pos_role, pos_target) > attackRange);
        }

    }
}

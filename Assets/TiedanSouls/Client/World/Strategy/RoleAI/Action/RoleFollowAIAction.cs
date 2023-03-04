using GameArki.BTTreeNS;
using TiedanSouls.World.Facades;
using UnityEngine;

namespace TiedanSouls.World.Entities {
    public class RoleFollowAIAction : IBTTreeAction {

        RoleEntity role;
        WorldContext worldContext;
        float attackRange;
        float sight;
        Vector2 dir;
        public RoleFollowAIAction() : this(1, 10) { }
        public RoleFollowAIAction(float attackRange) : this(attackRange, 10) { }
        public RoleFollowAIAction(float attackRange, float sight) {
            this.attackRange = attackRange;
            this.sight = sight;
        }

        public void Inject(RoleEntity role, WorldContext worldContext) {
            this.role = role;
            this.worldContext = worldContext;
        }

        public void Enter() {
            dir = Vector2.zero;
            TDLog.Log("RoleFollowAIAction Enter");
        }

        public bool Execute() {
            //OnDisable
            if (role.FSMCom.State == RoleFSMState.Dying) {
                TDLog.Warning("hp");
                return false;
            }

            //Miss Target
            Vector2 pos_role = role.GetPos_RendererRoot();
            Vector2 pos_target = worldContext.RoleRepo.PlayerRole.GetPos_RendererRoot();
            if (Vector2.Distance(pos_role, pos_target) >= sight) {
                TDLog.Warning("miss target");
                return false;
            }

            //Follow
            var input = role.InputCom;
            dir = (pos_target - pos_role).normalized;
            input.SetInput_Locomotion_Move(dir);
            return (Vector2.Distance(pos_role, pos_target) > attackRange);
        }

        public void Exit() {
            dir = Vector2.zero;
            TDLog.Log("RoleFollowAIAction Exit");
        }
    }
}
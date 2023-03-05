using GameArki.BTTreeNS;
using TiedanSouls.World.Facades;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleAttackAIAction : IBTTreeAction {
        RoleEntity role;
        WorldContext worldContext;
        float attackRange;
        float attackCD;
        float time;

        public RoleAttackAIAction() : this(2, 1) { }

        public RoleAttackAIAction(float attackRange) : this(attackRange, 1) { }

        public RoleAttackAIAction(float attackRange, float attackCD) {
            this.attackRange = attackRange;
            this.attackCD = attackCD;
        }

        public void Inject(RoleEntity role, WorldContext worldContext) {
            this.role = role;
            this.worldContext = worldContext;
        }

        public void Enter() {
            time = attackCD;
            TDLog.Log("RoleAttackAIAction Enter");
        }

        public bool Execute() {
            if (role.FSMCom.State == RoleFSMState.BeHit)return false;

            Vector2 pos_role = role.GetPos_RendererRoot();
            Vector2 pos_target = worldContext.RoleRepo.PlayerRole.GetPos_RendererRoot();
            if (Vector2.Distance(pos_role, pos_target) > attackRange) {
                return false;
            }

            //TODO:攻击方式后续修改，暂时先凑合一下
            if (time >= attackCD) {
                worldContext.RoleRepo.PlayerRole.HitBeHit(30);
                time = 0;
            }
            time += Time.deltaTime;
            return true;
        }

        public void Exit() {
            time = attackCD;
            TDLog.Log("RoleAttackAIAction Exit");
        }
    }

}
using GameArki.BTTreeNS;
using TiedanSouls.Client.Facades;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class RolePatrolAIAction : IBTTreeAction {

        RoleEntity role;
        WorldContext worldContext;
        Vector2 bfPos;
        Vector2 patrolPos1;
        Vector2 patrolPos2;
        Vector2 targetPos;
        float time;
        bool isCD;
        Vector2 dir;
        float sight;

        public RolePatrolAIAction() : this(10) {
        }

        public RolePatrolAIAction(float sight) {
            this.sight = sight;
            isCD = true;
        }

        public void Inject(RoleEntity role, WorldContext worldContext) {
            this.role = role;
            this.worldContext = worldContext;
        }

        void IBTTreeAction.Enter() {
            isCD = true;
            time = 0;
            targetPos = Vector2.zero;
            TDLog.Log("RolePatrolAIAction Enter");
        }

        bool IBTTreeAction.Execute() {
            //OnDeath
            if (role.AttributeCom.HP <=0) {
                return false;
            }

            //Check State
            Vector2 pos_role = role.LogicRootPos;
            Vector2 pos_target = worldContext.RoleRepo.PlayerRole.LogicRootPos;
            if (Vector2.Distance(pos_role, pos_target) < sight) {
                return false;
            }

            //Patrol CD
            if (isCD) {
                time += Time.deltaTime;
                if (time > 0.5f) {
                    isCD = false;
                    time = 0;
                }
                //TDLog.Log($"cd time:{time}");
                return true;
            }

            //Get Original Pos
            if (bfPos == Vector2.zero) {
                bfPos = role.LogicRootPos;
            }

            //Set Patrol Pos
            if (targetPos == Vector2.zero) {
                if (patrolPos1 == Vector2.zero || patrolPos2 == Vector2.zero) {
                    dir = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), 0);
                    patrolPos1 = bfPos + dir;
                    patrolPos2 = bfPos - dir;
                }
                bool patrol = Vector2.Distance(role.LogicRootPos, patrolPos1) >= Vector2.Distance(role.LogicRootPos, patrolPos2);
                targetPos = patrol ? patrolPos1 : patrolPos2;             
                //TDLog.Log($"dir:{dir},targetPos1:{patrolPos1},targetPos2:{patrolPos2}");
            }

            //Patrol
            var distance = Vector2.Distance(role.LogicRootPos, targetPos);
            if (distance <= 0.1f) {
                isCD = true;
                targetPos = Vector2.zero;
                return true;
            }
            var input = role.InputCom;
            dir = (targetPos - pos_role).normalized;
            input.SetMoveAxis(dir);

            return true;
        }

        void IBTTreeAction.Exit() {
            isCD = true;
            time = 0;
            targetPos = Vector2.zero;
            TDLog.Log("RolePatrolAIAction exit");
        }

    }

}
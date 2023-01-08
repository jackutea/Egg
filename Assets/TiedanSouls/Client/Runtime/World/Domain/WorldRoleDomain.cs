using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World.Domain {

    public class WorldRoleDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldRoleDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public int SpawnRole(Vector2 pos) {

            var idService = worldContext.IDService;
            var role = worldContext.WorldFactory.CreateRoleEntity(idService);
            role.SetPos(pos);

            role.OnCollisionEnterHandle += OnCollisionEnter;

            var repo = worldContext.RoleRepo;
            repo.Add(role);

            return role.ID;
        }

        void OnCollisionEnter(RoleEntity role, Collision2D other) {
            if (other.gameObject.layer == LayerCollection.GROUND) {
                role.EnterGround();
            }
        }

        public void RecordOwnerInput(RoleEntity ownerRole) {

            var inputGetter = infraContext.InputCore.Getter;
            var inputRecordCom = ownerRole.InputRecordCom;

            // - Move
            Vector2 moveAxis = Vector2.zero;
            if (inputGetter.GetPressing(InputKeyCollection.MOVE_LEFT)) {
                moveAxis.x = -1;
            } else if (inputGetter.GetPressing(InputKeyCollection.MOVE_RIGHT)) {
                moveAxis.x = 1;
            } else {
                moveAxis.x = 0;
            }
            inputRecordCom.SetMoveAxis(moveAxis);

            // - Jump
            bool isJump = inputGetter.GetPressing(InputKeyCollection.JUMP);
            inputRecordCom.SetJumping(isJump);

            // - Melee
            bool isMelee = inputGetter.GetPressing(InputKeyCollection.MELEE);
            inputRecordCom.SetMelee(isMelee);

            // - Crush
            bool isCrush = inputGetter.GetPressing(InputKeyCollection.CRUSH);
            inputRecordCom.SetCrush(isCrush);

        }

        public void Move(RoleEntity role) {
            role.Move();
        }

        public void Jump(RoleEntity role) {
            role.Jump();
        }

    }
}
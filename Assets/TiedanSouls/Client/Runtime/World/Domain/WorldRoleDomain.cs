using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Entities;
using TiedanSouls.Template;

namespace TiedanSouls.World.Domain {

    public class WorldRoleDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldRoleDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public RoleEntity SpawnRole(sbyte ally, Vector2 pos) {

            var idService = worldContext.IDService;
            var templateCore = infraContext.TemplateCore;

            var role = worldContext.WorldFactory.CreateRoleEntity(idService);

            // - Pos
            role.SetPos(pos);

            // - Ally
            role.SetAlly(ally);

            // - Skillor
            var skillor = new SkillorModel();
            bool has = templateCore.SkillorTemplate.TryGet(1000, out SkillorTM tm);
            if (!has) {
                TDLog.Error("Failed to get skillor template: 1000");
                return null;
            }
            skillor.FromTM(tm);
            role.SkillorSlotCom.Add(skillor);

            // - Physics
            role.OnCollisionEnterHandle += OnCollisionEnter;

            // - FSM
            role.FSMCom.EnterIdle();

            var repo = worldContext.RoleRepo;
            repo.Add(role);

            return role;
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

        public void Falling(RoleEntity role, float dt) {
            role.Falling(dt);
        }

    }
}
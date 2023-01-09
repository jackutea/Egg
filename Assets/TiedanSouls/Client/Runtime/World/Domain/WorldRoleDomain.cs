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

        public RoleEntity SpawnRole(int typeID, sbyte ally, Vector2 pos) {

            var idService = worldContext.IDService;
            var templateCore = infraContext.TemplateCore;

            // - Entity
            var worldAssets = infraContext.AssetCore.WorldAssets;
            bool has = worldAssets.TryGet("entity_role", out GameObject go);
            if (!has) {
                TDLog.Error("Failed to get asset: entity_role");
                return null;
            }

            var role = GameObject.Instantiate(go).GetComponent<RoleEntity>();
            role.Ctor();
            
            // - ID
            int id = idService.PickRoleID();
            role.SetID(id);

            // - TM
            has = templateCore.RoleTemplate.TryGet(typeID, out RoleTM roleTM);
            if (!has) {
                TDLog.Error("Failed to get role template: " + typeID);
                return null;
            }

            // - Mesh
            var spriteAssets = infraContext.AssetCore.SpriteAssets;
            has = spriteAssets.TryGet(roleTM.meshName, out Sprite sprite);
            if (!has) {
                TDLog.Error("Failed to get sprite: " + roleTM.meshName);
                return null;
            }
            role.SetMesh(sprite);

            // - Move
            var moveCom = role.MoveCom;
            moveCom.Initialize(roleTM.moveSpeed, roleTM.jumpSpeed, roleTM.fallingAcceleration, roleTM.fallingSpeedMax);

            // - Pos
            role.SetPos(pos);

            // - Ally
            role.SetAlly(ally);

            // - Skillor
            if (roleTM.skillorTypeIDArray!= null) {
                foreach (var skillorTypeID in roleTM.skillorTypeIDArray) {
                    var skillor = new SkillorModel();
                    has = templateCore.SkillorTemplate.TryGet(skillorTypeID, out SkillorTM skillorTM);
                    if (!has) {
                        TDLog.Error("Failed to get skillor template: " + skillorTypeID);
                        return null;
                    }
                    skillor.FromTM(skillorTM);
                    role.SkillorSlotCom.Add(skillor);
                }
            }

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
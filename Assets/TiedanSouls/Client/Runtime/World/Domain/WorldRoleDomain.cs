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

            var factory = worldContext.WorldFactory;
            var role = factory.CreateRoleEntity(typeID, ally, pos);

            // ==== Init ====
            // - Skillor 
            var allSkillor = role.SkillorSlotCom.GetAll();
            foreach (var skillor in allSkillor) {
                skillor.OnTriggerEnterHandle += OnSkillorTriggerEnter;
            }

            // - Physics
            role.OnFootCollisionEnterHandle += OnRoleFootCollisionEnter;
            role.OnFootCollisionExitHandle += OnRoleFootCollisionExit;
            role.OnBodyTriggerExitHandle += OnRoleFootTriggerExit;

            // - FSM
            role.FSMCom.EnterIdle();

            var repo = worldContext.RoleRepo;
            repo.Add(role);

            return role;
        }

        // ==== Physics Event ====
        void OnRoleFootCollisionEnter(RoleEntity role, Collision2D other) {
            if (other.gameObject.layer == LayerCollection.GROUND) {
                role.EnterGround();
            } else if (other.gameObject.layer == LayerCollection.CROSS_PLATFORM) {
                role.EnterCrossPlatform();
            }
        }

        void OnRoleFootCollisionExit(RoleEntity role, Collision2D other) {
            if (other.gameObject.layer == LayerCollection.GROUND) {
                role.LeaveGround();
            } else if (other.gameObject.layer == LayerCollection.CROSS_PLATFORM) {
                role.LeaveCrossPlatform();
            }
        }

        void OnRoleFootTriggerExit(RoleEntity role, Collider2D other) {
            if (other.gameObject.layer == LayerCollection.CROSS_PLATFORM) {
                role.LeaveCrossPlatform();
            }
        }

        void OnSkillorTriggerEnter(SkillorModel skillor, Collider2D other) {
            var go = other.gameObject;
            var otherRole = go.GetComponent<RoleEntity>();
            if (otherRole != null) {
                RoleHitRole(skillor, otherRole);
            }
        }

        // ==== Input ====
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

            if (inputGetter.GetPressing(InputKeyCollection.MOVE_DOWN)) {
                moveAxis.y = -1;
            } else if (inputGetter.GetPressing(InputKeyCollection.MOVE_UP)) {
                moveAxis.y = 1;
            } else {
                moveAxis.y = 0;
            }
            inputRecordCom.SetMoveAxis(moveAxis);

            // - Jump
            bool isJump = inputGetter.GetDown(InputKeyCollection.JUMP);
            inputRecordCom.SetJumping(isJump);

            // - Melee && HoldMelee
            bool isMelee = inputGetter.GetDown(InputKeyCollection.MELEE);
            inputRecordCom.SetMelee(isMelee);

            // - SpecMelee
            bool isSpecMelee = inputGetter.GetDown(InputKeyCollection.SPEC_MELEE);
            inputRecordCom.SetSpecMelee(isSpecMelee);

            // - BoomMelee
            bool isBoomMelee = inputGetter.GetDown(InputKeyCollection.BOOM_MELEE);
            inputRecordCom.SetBoomMelee(isBoomMelee);

            // - Infinity
            bool isInfinity = inputGetter.GetDown(InputKeyCollection.INFINITY);
            inputRecordCom.SetInfinity(isInfinity);

            // - Dash
            bool isDash = inputGetter.GetDown(InputKeyCollection.DASH);
            inputRecordCom.SetDash(isDash);

        }

        // ==== Locomotion ====
        public void Move(RoleEntity role) {
            role.Move();
        }

        public void Dash(RoleEntity role, Vector2 dir, Vector2 force) {
            role.Dash(dir, force);
        }

        public void Jump(RoleEntity role) {
            role.Jump();
        }

        public void CrossDown(RoleEntity role) {
            role.CrossDown();
        }

        public void Falling(RoleEntity role, float dt) {
            role.Falling(dt);
        }

        // ==== Cast ====
        public void CastByInput(RoleEntity role) {

            // Allowed When Idle
            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.Idle) {
                return;
            }

            // Not Allowed When Dead || Hurt
            // Need Cancel When Casting

            var inputRecordCom = role.InputRecordCom;
            if (inputRecordCom.IsSpecMelee) {
                CastByType(role, SkillorType.SpecMelee);
            } else if (inputRecordCom.IsBoomMelee) {
                CastByType(role, SkillorType.BoomMelee);
            } else if (inputRecordCom.IsInfinity) {
                CastByType(role, SkillorType.Infinity);
            } else if (inputRecordCom.IsDash) {
                CastByType(role, SkillorType.Dash);
            } else if (inputRecordCom.IsMelee) {
                CastByType(role, SkillorType.Melee);
            }
        }

        void CastByType(RoleEntity role, SkillorType skillorType) {
            var skillorSlotCom = role.SkillorSlotCom;
            bool has = skillorSlotCom.TryGetByType(skillorType, out var skillor);
            if (!has) {
                TDLog.Error("Failed to get skillor: " + skillorType);
                return;
            }

            // TDLog.Log("Cast: " + skillorType + " -> " + role.ID + " -> " + skillor.TypeID);

            Cast(role, skillor);
        }

        void Cast(RoleEntity role, SkillorModel skillor) {
            var fsm = role.FSMCom;
            fsm.EnterCasting(skillor);
        }

        // ==== Hit ====
        void RoleHitRole(SkillorModel skillor, RoleEntity other) {

            var caster = skillor.Owner;

            // Me Check
            if (caster == other) {
                return;
            }

            // Ally Check
            if (caster.Ally == other.Ally) {
                return;
            }

            // Damage Arbit: Prevent Multi Hit
            var damageArbitService = worldContext.DamageArbitService;
            if (damageArbitService.IsInArbit(skillor.EntityType, skillor.ID, other.EntityType, other.ID)) {
                return;
            }
            damageArbitService.TryAdd(skillor.EntityType, skillor.ID, other.EntityType, other.ID);

            RoleHitRole_Damage(caster, skillor, other);
            if (other.AttrCom.HP <= 0) {
                RoleDie(other);
            } else {
                RoleHitRole_FrameEffector(caster, skillor, other);
            }

        }

        void RoleHitRole_Damage(RoleEntity caster, SkillorModel skillor, RoleEntity other) {

            // Weapon Damage
            var curWeapon = caster.WeaponSlotCom.Weapon;
            other.HitBeHurt(curWeapon.atk);

        }

        void RoleHitRole_FrameEffector(RoleEntity caster, SkillorModel skillor, RoleEntity other) {

            // Frame Effector
            bool hasFrame = skillor.TryGetCurrentFrame(out var frame);
            if (!hasFrame) {
                TDLog.Error("Failed to get frame");
                return;
            }

            SkillorFrameElement otherFrame = null;
            var otherFSM = other.FSMCom;
            if (otherFSM.Status == RoleFSMStatus.Casting) {
                var otherSkillor = otherFSM.CastingState.castingSkillor;
                otherSkillor.TryGetCurrentFrame(out otherFrame);
            }

            var hitPower = frame.hitPower;
            if (frame.hitPower != null) {

                if (otherFrame != null && hitPower.breakPowerLevel < otherFrame.hitPower.sufferPowerLevel) {
                    // Not Break
                    return;
                }

                otherFSM.EnterBeHurt(caster.GetPos(), hitPower);

            }

        }

        void RoleDie(RoleEntity role) {
            var fsm = role.FSMCom;
            fsm.EnterDead();
        }

    }
}
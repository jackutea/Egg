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

        public RoleEntity SpawnRole(RoleControlType controlType, int typeID, AllyType allyType, Vector2 pos) {

            var factory = worldContext.WorldFactory;
            var role = factory.SpawnRoleEntity(controlType, typeID, allyType, pos);

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

            // - AI
            if (controlType == RoleControlType.AI) {
                var ai = role.AIStrategy;
                ai.Activate();
            }

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
            if (!inputRecordCom.IsMelee) {
                inputRecordCom.SetMelee(isMelee);
            }

            // - SpecMelee
            bool isSpecMelee = inputGetter.GetDown(InputKeyCollection.SPEC_MELEE);
            if (!inputRecordCom.IsSpecMelee) {
                inputRecordCom.SetSpecMelee(isSpecMelee);
            }

            // - BoomMelee
            bool isBoomMelee = inputGetter.GetDown(InputKeyCollection.BOOM_MELEE);
            if (!inputRecordCom.IsBoomMelee) {
                inputRecordCom.SetBoomMelee(isBoomMelee);
            }

            // - Infinity
            bool isInfinity = inputGetter.GetDown(InputKeyCollection.INFINITY);
            if (!inputRecordCom.IsInfinity) {
                inputRecordCom.SetInfinity(isInfinity);
            }

            // - Dash
            bool isDash = inputGetter.GetDown(InputKeyCollection.DASH);
            if (!inputRecordCom.IsDash) {
                inputRecordCom.SetDash(isDash);
            }

        }

        public void CleanOwnerInput(RoleEntity ownerRole) {
            var inputRecordCom = ownerRole.InputRecordCom;
            inputRecordCom.SetMelee(false);
            inputRecordCom.SetSpecMelee(false);
            inputRecordCom.SetBoomMelee(false);
            inputRecordCom.SetInfinity(false);
            inputRecordCom.SetDash(false);
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
        public bool CastByInput(RoleEntity role) {

            // - Get Input TypeID
            var inputRecordCom = role.InputRecordCom;
            SkillorType inputSkillorType = SkillorType.None;
            if (inputRecordCom.IsSpecMelee) {
                inputSkillorType = SkillorType.SpecMelee;
            } else if (inputRecordCom.IsBoomMelee) {
                inputSkillorType = SkillorType.BoomMelee;
            } else if (inputRecordCom.IsInfinity) {
                inputSkillorType = SkillorType.Infinity;
            } else if (inputRecordCom.IsDash) {
                inputSkillorType = SkillorType.Dash;
            } else if (inputRecordCom.IsMelee) {
                inputSkillorType = SkillorType.Melee;
            }

            if (inputSkillorType == SkillorType.None) {
                return false;
            }

            if (!role.SkillorSlotCom.TryGetOriginalSkillorByType(inputSkillorType, out var skillor)) {
                TDLog.Error($"释放技能失败 - 不存在原始技能类型 {inputSkillorType}");
                return false;
            }

            int inputSkillorTypeID = skillor.TypeID;

            // - Judge Allow Cast
            bool allowCast = AllowCast(role, ref inputSkillorTypeID, out var isCombo);
            if (!allowCast) {
                return false;
            }

            // - Cast
            if (isCombo) {
                CastComboSkillor(role, inputSkillorTypeID);
            } else {
                CastOriginalSkillor(role, inputSkillorTypeID);
            }

            return true;

        }

        bool AllowCast(RoleEntity role, ref int inputSkillorTypeID, out bool isCombo) {
            isCombo = false;

            var fsm = role.FSMCom;

            if (fsm.Status == RoleFSMStatus.Idle) {
                return true;
            }

            // - Skillor Cancel
            if (fsm.Status == RoleFSMStatus.Casting) {

                var castingSkillor = fsm.CastingState.castingSkillor;
                if (!castingSkillor.TryGetCurrentFrame(out var frame)) {
                    TDLog.Error($"技能未配置帧 - {castingSkillor.TypeID} ");
                    return false;
                }

                var cancels = frame.cancels;
                if (cancels == null || cancels.Length == 0) {
                    return false;
                }

                var skillorSlotCom = role.SkillorSlotCom;
                for (int i = 0; i < cancels.Length; i += 1) {
                    SkillorCancelModel cancel = cancels[i];
                    int cancelSkillorTypeID = cancel.skillorTypeID;

                    if (cancel.isCombo) {
                        // - Combo Cancel
                        isCombo = true;
                        if (!skillorSlotCom.TryGetComboSkillor(cancelSkillorTypeID, out var comboSkillor)
                        || inputSkillorTypeID != comboSkillor.OriginalSkillorTypeID) {
                            return false;
                        }

                        inputSkillorTypeID = comboSkillor.TypeID;
                        return true;
                    } else {
                        // - Normal Cancel
                        if (cancelSkillorTypeID == inputSkillorTypeID) {
                            if (skillorSlotCom.TryGetOriginalSkillor(cancelSkillorTypeID, out var nextSkillor)) {
                                return true;
                            } else {
                                TDLog.Error($"技能配置错误 - 强制取消技能 ID {cancelSkillorTypeID}");
                                return false;
                            }
                        }
                    }

                }

            }

            return false;

        }

        void CastOriginalSkillor(RoleEntity role, int typeID) {
            var skillorSlotCom = role.SkillorSlotCom;
            if (!skillorSlotCom.TryGetOriginalSkillor(typeID, out var skillor)) {
                TDLog.Error($"释放原始技能失败:{typeID} ");
                return;
            }
            var fsm = role.FSMCom;
            fsm.EnterCasting(skillor, false);
        }

        void CastComboSkillor(RoleEntity role, int typeID) {
            var skillorSlotCom = role.SkillorSlotCom;
            if (!skillorSlotCom.TryGetComboSkillor(typeID, out var skillor)) {
                TDLog.Error($"释放连击技能失败:{typeID} ");
                return;
            }
            var fsm = role.FSMCom;
            fsm.EnterCasting(skillor, true);
        }

        // ==== Hit ====
        void RoleHitRole(SkillorModel skillor, RoleEntity other) {

            var caster = skillor.Owner;

            // Me Check
            if (caster == other) {
                return;
            }

            // Ally Check
            if (caster.AllyType == other.AllyType) {
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

        public void TickHUD(RoleEntity role, float dt) {
            if (role.HudSlotCom.HpBarHUD != null) {
                role.HudSlotCom.HpBarHUD.Tick(dt);
            }
        }

    }

}
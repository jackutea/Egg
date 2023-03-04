using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Entities;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.World.Domain {

    public class WorldRoleDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldRoleDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public RoleEntity SpawnRole(ControlType controlType, int typeID, AllyType allyType, Vector2 pos) {

            var factory = worldContext.WorldFactory;
            var role = factory.SpawnRoleEntity(controlType, typeID, allyType, pos);

            // - Physics
            role.FootTriggerEnterAction += OnFootTriggerEnter;
            role.FootTriggerExit += OnFootTriggerExit;

            // - FSM
            role.FSMCom.EnterIdle();

            var repo = worldContext.RoleRepo;
            if (role.ControlType == ControlType.Player) {
                repo.SetPlayerRole(role);
            } else if (role.ControlType == ControlType.AI) {
                var ai = role.AIStrategy;
                ai.Activate();
                var fromFieldTypeID = worldContext.StateEntity.CurFieldTypeID;
                repo.AddAIRole(role, fromFieldTypeID);
            }

            return role;
        }

        #region [Physics Event]

        void OnFootTriggerEnter(RoleEntity role, Collider2D other) {
            if (other.gameObject.layer == LayerCollection.GROUND) {
                role.EnterGround();
            } else if (other.gameObject.layer == LayerCollection.CROSS_PLATFORM) {
                role.EnterCrossPlatform();
            }
        }

        void OnFootTriggerExit(RoleEntity role, Collider2D other) {
            if (other.gameObject.layer == LayerCollection.GROUND) {
                role.LeaveGround();
            } else if (other.gameObject.layer == LayerCollection.CROSS_PLATFORM) {
                role.LeaveCrossPlatform();
            }
        }

        void OnTriggerEnter_Skillor(SkillorModel skillor, Collider2D other) {
            var go = other.gameObject;
            var otherRole = go.GetComponentInParent<RoleEntity>();
            if (otherRole != null) {
                SkillorHitRole(skillor, otherRole);
            }
        }

        #endregion

        // ==== Input ====
        public void BackPlayerRInput() {
            RoleEntity playerRole = worldContext.RoleRepo.PlayerRole;
            if (playerRole == null) {
                return;
            }

            var inputCom = playerRole.InputCom;
            var inputGetter = infraContext.InputCore.Getter;

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
            inputCom.SetInput_Locomotion_Move(moveAxis);

            // - Jump
            if (inputGetter.GetDown(InputKeyCollection.JUMP)) {
                inputCom.SetInput_Locomotion_Jump(true);
            }

            // - Skillor Melee && HoldMelee
            if (inputGetter.GetDown(InputKeyCollection.MELEE)) {
                inputCom.SetInput_Skillor__Melee(true);
            }

            // - Skillor SpecMelee
            if (inputGetter.GetDown(InputKeyCollection.SPEC_MELEE)) {
                inputCom.SetInput_Skillor_SpecMelee(true);
            }

            // - Skillor BoomMelee
            if (inputGetter.GetDown(InputKeyCollection.BOOM_MELEE)) {
                inputCom.SetInput_Skillor_BoomMelee(true);
            }

            // - Skillor Infinity
            if (inputGetter.GetDown(InputKeyCollection.INFINITY)) {
                inputCom.SetInput_Skillor_Infinity(true);
            }

            // - Skillor Dash
            if (inputGetter.GetDown(InputKeyCollection.DASH)) {
                inputCom.SetInput_Skillor_Dash(true);
            }

            // - Pick
            if (inputGetter.GetDown(InputKeyCollection.PICK)) {
                inputCom.SetInput_Basic_Pick(true);
            }

            // - Choose Point
            if (inputGetter.GetDown(InputKeyCollection.CHOOSE_POINT)) {
                inputCom.SetInput_Basic_ChoosePoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        #region [Locomotion]

        public void Move(RoleEntity role) {
            role.Move();
        }

        public void SetRoleFaceDirX(RoleEntity role, sbyte dirX) {
            role.SetFaceDirX(dirX);
        }

        public void Dash(RoleEntity role, Vector2 dir, Vector2 force) {
            role.Dash(dir, force);
        }

        public void Jump(RoleEntity role) {
            role.Jump();
        }

        public void CrossDown(RoleEntity role) {
            role.TryCrossDown();
        }

        public void Falling(RoleEntity role, float dt) {
            if (role.MoveCom.IsGrounded) {
                return;
            }

            role.Falling(dt);
        }

        #endregion

        #region [Cast]

        public bool TryCastSkillorByInput(RoleEntity role) {
            var inputCom = role.InputCom;
            SkillorType inputSkillorType = inputCom.GetSkillorType();
            if (inputSkillorType == SkillorType.None) {
                return false;
            }

            var weaponSlotCom = role.WeaponSlotCom;
            if (!weaponSlotCom.IsActive) {
                TDLog.Warning($"无法施放技能 - 武器未激活");
                return false;
            }

            if (!role.SkillorSlotCom.TryGetOriginalSkillorByType(inputSkillorType, out var skillor)) {
                TDLog.Error($"施放技能失败 - 不存在原始技能类型 {inputSkillorType}");
                return false;
            }

            int inputSkillorTypeID = skillor.TypeID;

            var fsm = role.FSMCom;
            if (fsm.Status == RoleFSMStatus.Idle) {
                CastOriginalSkillor(role, inputSkillorTypeID);
                return true;
            }

            if (CanCancelSkillor(role, inputSkillorTypeID, out var realSkillorTypeID, out var isCombo)) {
                if (isCombo) {
                    CastComboSkillor(role, realSkillorTypeID);
                } else {
                    CastOriginalSkillor(role, realSkillorTypeID);
                }
            }

            return true;
        }

        bool CanCancelSkillor(RoleEntity role, int inputSkillorTypeID, out int realSkillorTypeID, out bool isCombo) {
            realSkillorTypeID = inputSkillorTypeID;
            isCombo = false;

            // - Skillor Cancel
            var fsm = role.FSMCom;
            if (fsm.Status == RoleFSMStatus.Casting) {

                var stateModel = fsm.CastingState;
                var skillID = stateModel.skillorTypeID;
                SkillorModel castingSkillor;
                if (stateModel.isCombo) {
                    role.SkillorSlotCom.TryGetComboSkillor(skillID, out castingSkillor);
                } else {
                    role.SkillorSlotCom.TryGetOriginalSkillorByTypeID(skillID, out castingSkillor);
                }

                if (!castingSkillor.TryGetCurrentFrame(out var frame)) {
                    TDLog.Error($"技能未配置帧 - {castingSkillor.TypeID} ");
                    return false;
                }

                var allCancelModels = frame.allCancelModels;
                if (allCancelModels == null || allCancelModels.Length == 0) {
                    return false;
                }

                var skillorSlotCom = role.SkillorSlotCom;
                for (int i = 0; i < allCancelModels.Length; i++) {
                    SkillorCancelModel cancel = allCancelModels[i];
                    int cancelSkillorTypeID = cancel.skillorTypeID;

                    if (cancel.isCombo) {
                        // - Combo Cancel
                        bool hasCombo = skillorSlotCom.TryGetComboSkillor(cancelSkillorTypeID, out var comboSkillor);
                        bool isComboInput = inputSkillorTypeID == comboSkillor.OriginalSkillorTypeID;
                        if (hasCombo && isComboInput) {
                            realSkillorTypeID = cancelSkillorTypeID;    // 改变技能类型ID
                            isCombo = true;
                            return true;
                        }
                    } else {
                        // - Normal Cancel
                        bool hasOriginal = skillorSlotCom.TryGetOriginalSkillorByTypeID(cancelSkillorTypeID, out var originalSkillor);
                        bool isOriginalInput = inputSkillorTypeID == cancelSkillorTypeID;
                        if (hasOriginal && isOriginalInput) {
                            isCombo = false;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        void CastOriginalSkillor(RoleEntity role, int typeID) {
            var skillorSlotCom = role.SkillorSlotCom;
            if (!skillorSlotCom.TryGetOriginalSkillorByTypeID(typeID, out var skillor)) {
                TDLog.Error($"施放原始技能失败:{typeID} ");
                return;
            }
            var fsm = role.FSMCom;
            fsm.EnterCasting(skillor, false);
        }

        void CastComboSkillor(RoleEntity role, int typeID) {
            var skillorSlotCom = role.SkillorSlotCom;
            if (!skillorSlotCom.TryGetComboSkillor(typeID, out var skillor)) {
                TDLog.Error($"施放连击技能失败:{typeID} ");
                return;
            }
            var fsm = role.FSMCom;
            fsm.EnterCasting(skillor, true);
        }

        #endregion  

        #region [Hit]

        void SkillorHitRole(SkillorModel skillor, RoleEntity other) {

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
            if (damageArbitService.IsInArbit(skillor.EntityType, skillor.ID, other.EntityType, other.EntityD)) {
                return;
            }
            damageArbitService.TryAdd(skillor.EntityType, skillor.ID, other.EntityType, other.EntityD);

            SkillorHitRole_Damage(caster, skillor, other);
            if (other.AttrCom.HP <= 0) {
                RoleDie(other);
            } else {
                RoleHitRole_FrameEffector(caster, skillor, other);
            }

        }

        void SkillorHitRole_Damage(RoleEntity caster, SkillorModel skillor, RoleEntity other) {

            // Weapon Damage
            var curWeapon = caster.WeaponSlotCom.Weapon;
            other.HitBeHit(curWeapon.atk);

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
                var stateModel = otherFSM.CastingState;
                var skillID = stateModel.skillorTypeID;
                var isCombo = stateModel.isCombo;
                SkillorModel otherSkillor;
                if (isCombo) {
                    other.SkillorSlotCom.TryGetComboSkillor(skillID, out otherSkillor);
                } else {
                    other.SkillorSlotCom.TryGetOriginalSkillorByTypeID(skillID, out otherSkillor);
                }
                otherSkillor.TryGetCurrentFrame(out otherFrame);
            }

            var hitPower = frame.hitPower;
            if (frame.hitPower != null) {

                if (otherFrame != null && hitPower.breakPowerLevel < otherFrame.hitPower.sufferPowerLevel) {
                    // Not Break
                    return;
                }

                otherFSM.EnterBeHit(caster.GetPos_Logic(), hitPower);

            }

        }

        #endregion

        void RoleDie(RoleEntity role) {
            var fsm = role.FSMCom;
            fsm.EnterDead();
        }

        #region [拾取武器 -> 初始化武器组件 -> 添加对应技能]

        public bool TryPickUpSomethingFromField(RoleEntity role) {
            var repo = worldContext.ItemRepo;
            var fieldTypeID = worldContext.StateEntity.CurFieldTypeID;
            if (!repo.TryGetOneItemFromField(fieldTypeID, role.GetPos_Logic(), 1, out var item)) {
                return false;
            }

            if (item.ItemType == ItemType.Weapon) {
                if (!role.WeaponSlotCom.IsActive) {
                    PickUpWeapon(role, item.TypeIDForPickUp);
                    return true;
                }

                // TODO: 拾取武器时，如果已经有武器，需要判断是否替换武器
                return false;
            }

            TDLog.Error($"未知的拾取物品类型 - {item.ItemType}");
            return false;
        }

        public void PickUpWeapon(RoleEntity role, int weaponTypeID) {
            // Weapon
            SetWeaponSlotComponent(role, weaponTypeID);

            // Skillor
            var curWeapon = role.WeaponSlotCom.Weapon;
            var skillorTypeIDArray = new int[] { curWeapon.skillorMeleeTypeID, curWeapon.skillorHoldMeleeTypeID, curWeapon.skillorSpecMeleeTypeID };
            if (skillorTypeIDArray != null) {
                InitRoleSkillorSlotCom(role, skillorTypeIDArray);
            }

            role.SkillorSlotCom.ForeachAllOriginalSkillor((skillor) => {
                TDLog.Log($"添加技能触发器 - {skillor.TypeID}");
                skillor.OnTriggerEnterHandle += OnTriggerEnter_Skillor;
            });

            role.SkillorSlotCom.ForeachAllComboSkillor((skillor) => {
                TDLog.Log($"添加技能触发器 - {skillor.TypeID}");
                skillor.OnTriggerEnterHandle += OnTriggerEnter_Skillor;
            });
        }

        void SetWeaponSlotComponent(RoleEntity role, int weaponTypeID) {
            var weaponModel = SpawnWeaponModel(weaponTypeID);
            if (weaponModel == null) {
                TDLog.Error($"武器生成失败 - {weaponTypeID}");
                return;
            }

            var mod = weaponModel.Mod;
            mod.transform.SetParent(role.WeaponSlotCom.WeaponRoot, false);
            role.WeaponSlotCom.SetWeapon(weaponModel);
        }

        WeaponEntity SpawnWeaponModel(int typeID) {
            WeaponEntity weapon = new WeaponEntity();

            var assetCore = infraContext.AssetCore;
            var templateCore = infraContext.TemplateCore;

            // Weapon TM
            bool has = templateCore.WeaponTemplate.TryGet(typeID, out WeaponTM weaponTM);
            if (!has) {
                TDLog.Error("Failed to get weapon template: " + typeID);
                return null;
            }

            // Weapon Mod
            has = assetCore.WeaponModAssets.TryGet(weaponTM.meshName, out GameObject weaponModPrefab);
            if (!has) {
                TDLog.Error("Failed to get weapon mod: " + weaponTM.meshName);
                return null;
            }

            weapon.SetWeaponType(weaponTM.weaponType);
            weapon.SetTypeID(weaponTM.typeID);
            weapon.atk = weaponTM.atk;
            weapon.def = weaponTM.def;
            weapon.crit = weaponTM.crit;
            weapon.skillorMeleeTypeID = weaponTM.skillorMeleeTypeID;
            weapon.skillorHoldMeleeTypeID = weaponTM.skillorHoldMeleeTypeID;
            weapon.skillorSpecMeleeTypeID = weaponTM.skillorSpecMeleeTypeID;

            var go = GameObject.Instantiate(weaponModPrefab);
            weapon.SetMod(go);

            return weapon;
        }

        void InitRoleSkillorSlotCom(RoleEntity role, int[] typeIDArray) {
            var templateCore = infraContext.TemplateCore;
            var idService = worldContext.IDService;
            var skillorSlotCom = role.SkillorSlotCom;

            var len = typeIDArray.Length;
            for (int i = 0; i < len; i++) {
                var typeID = typeIDArray[i];
                if (!templateCore.SkillorTemplate.TryGet(typeID, out SkillorTM skillorTM)) {
                    continue;
                }

                var skillorModel = new SkillorModel();
                skillorModel.FromTM(skillorTM);
                skillorModel.SetID(idService.PickSkillorID());
                skillorModel.SetOwner(role);
                skillorSlotCom.AddOriginalSkillor(skillorModel);

                var frames = skillorTM.frames;
                var frameCout = frames.Length;
                for (int j = 0; j < frameCout; j++) {
                    InitAllComboSkillor(role, skillorSlotCom, frames[j]);
                }
            }
        }

        void InitAllComboSkillor(object owner, SkillorSlotComponent skillorSlotCom, SkillorFrameTM frameTM) {
            var templateCore = infraContext.TemplateCore;
            var idService = worldContext.IDService;

            var cancelTMs = frameTM.cancelTMs;
            var cancelCount = cancelTMs?.Length;
            for (int i = 0; i < cancelCount; i++) {
                var cancelTM = cancelTMs[i];
                if (!cancelTM.isCombo) {
                    continue;
                }

                var comboSkillorTypeID = cancelTM.skillorTypeID;
                if (skillorSlotCom.HasComboSkillor(comboSkillorTypeID)) {
                    continue;
                }

                if (!templateCore.SkillorTemplate.TryGet(comboSkillorTypeID, out SkillorTM comboSkillorTM)) {
                    TDLog.Error($"加载连击技能失败 - TypeID {comboSkillorTypeID} 不存在 ");
                    continue;
                }

                var comboSkillorModel = new SkillorModel();
                comboSkillorModel.FromTM(comboSkillorTM);
                comboSkillorModel.SetID(idService.PickSkillorID());
                comboSkillorModel.SetOwner(owner);
                skillorSlotCom.AddComboSkillor(comboSkillorModel);

                var nextFrames = comboSkillorTM.frames;
                var frameCount = nextFrames.Length;
                for (int j = 0; j < frameCount; j++) {
                    InitAllComboSkillor(owner, skillorSlotCom, nextFrames[j]);
                }
            }
        }

        #endregion

    }

}
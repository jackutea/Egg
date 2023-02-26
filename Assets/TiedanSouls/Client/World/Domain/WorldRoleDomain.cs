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

            if (role.ControlType == RoleControlType.Player) {
                repo.playerRole = role;
            }

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
        public void BackPlayerRInput() {
            RoleEntity playerRole = worldContext.RoleRepo.playerRole;
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

            // - Pick
            if (inputGetter.GetDown(InputKeyCollection.PICK)) {
                inputCom.SetInput_Basic_Pick(true);
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
        public bool TryCancelSkillor(RoleEntity role) {
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

            if (!CanCast(role, ref inputSkillorTypeID, out var isCombo)) {
                return false;
            }

            if (isCombo) {
                CastComboSkillor(role, inputSkillorTypeID);
            } else {
                CastOriginalSkillor(role, inputSkillorTypeID);
            }

            return true;

        }

        bool CanCast(RoleEntity role, ref int inputSkillorTypeID, out bool isCombo) {
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
                            TDLog.Error($"强制取消技能失败 - 连击技能 {cancelSkillorTypeID} 不存在 或 非对应原始技能 {inputSkillorTypeID}");
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
                                TDLog.Error($"强制取消技能失败 - NormalCancel技能 {cancelSkillorTypeID} 未配置为原始技能");
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

        #region [拾取武器 -> 初始化武器组件 -> 添加对应技能]

        public bool TryPickUpSomething(RoleEntity role) {
            var repo = worldContext.ItemRepo;
            if (!repo.TryGetOneItem(role.GetPos(), 1, out var item)) {
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

            var allSkillor = role.SkillorSlotCom.GetAll();
            foreach (var skillor in allSkillor) {
                skillor.OnTriggerEnterHandle += OnSkillorTriggerEnter;
            }
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

        WeaponModel SpawnWeaponModel(int typeID) {
            WeaponModel weapon = new WeaponModel();

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
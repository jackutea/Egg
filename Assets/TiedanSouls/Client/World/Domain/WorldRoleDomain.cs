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

        #region [拾取武器 -> 初始化武器组件 -> 添加对应技能]

        public bool TryPickUpSomethingFromField(RoleEntity role) {
            var repo = worldContext.ItemRepo;
            var fieldTypeID = worldContext.StateEntity.CurFieldTypeID;
            if (!repo.TryGetOneItemFromField(fieldTypeID, role.GetPos_Logic(), 1, out var item)) {
                return false;
            }

            if (item.ItemType == ItemType.Weapon) {
                // TODO: 拾取武器时，如果已经有武器，需要判断是否替换武器
                if (!role.WeaponSlotCom.HasWeapon()) {
                    PickUpWeapon(role, item.TypeIDForPickUp);
                    return true;
                }

                return false;
            }

            TDLog.Error($"未知的拾取物品类型 - {item.ItemType}");
            return false;
        }

        public void PickUpWeapon(RoleEntity role, int weaponTypeID) {
            // Weapon
            SetWeaponSlotComponent(role, weaponTypeID);

            // Skill
            var curWeapon = role.WeaponSlotCom.Weapon;
            var skillTypeIDArray = new int[] { curWeapon.skillMeleeTypeID, curWeapon.skillHoldMeleeTypeID, curWeapon.skillSpecMeleeTypeID };
            if (skillTypeIDArray != null) {
                InitRoleSkillSlotCom(role, skillTypeIDArray);
            }

            role.SkillSlotCom.ForeachAllOriginalSkill((skill) => {
                TDLog.Log($"添加技能触发器 - {skill.TypeID}");
                skill.OnTriggerEnterHandle += OnTriggerEnter_Skill;
            });

            role.SkillSlotCom.ForeachAllComboSkill((skill) => {
                TDLog.Log($"添加技能触发器 - {skill.TypeID}");
                skill.OnTriggerEnterHandle += OnTriggerEnter_Skill;
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
            weapon.skillMeleeTypeID = weaponTM.skillMeleeTypeID;
            weapon.skillHoldMeleeTypeID = weaponTM.skillHoldMeleeTypeID;
            weapon.skillSpecMeleeTypeID = weaponTM.skillSpecMeleeTypeID;

            var go = GameObject.Instantiate(weaponModPrefab);
            weapon.SetMod(go);

            return weapon;
        }

        void InitRoleSkillSlotCom(RoleEntity role, int[] typeIDArray) {
            var templateCore = infraContext.TemplateCore;
            var idService = worldContext.IDService;
            var skillSlotCom = role.SkillSlotCom;

            var len = typeIDArray.Length;
            for (int i = 0; i < len; i++) {
                var typeID = typeIDArray[i];
                if (!templateCore.SkillTemplate.TryGet(typeID, out SkillTM skillTM)) {
                    continue;
                }

                var skillModel = new SkillModel();
                skillModel.FromTM(skillTM);
                skillModel.SetID(idService.PickSkillID());
                skillModel.SetOwner(role);
                skillSlotCom.AddOriginalSkill(skillModel);
            }
        }

        void InitAllComboSkill(object owner, SkillSlotComponent skillSlotCom, SkillFrameTM frameTM) {
            var templateCore = infraContext.TemplateCore;
            var idService = worldContext.IDService;

            var cancelTMs = frameTM.cancelTMs;
            var cancelCount = cancelTMs?.Length;
            for (int i = 0; i < cancelCount; i++) {
                var cancelTM = cancelTMs[i];
                if (!cancelTM.isCombo) {
                    continue;
                }

                var comboSkillTypeID = cancelTM.skillTypeID;
                if (skillSlotCom.HasComboSkill(comboSkillTypeID)) {
                    continue;
                }

                if (!templateCore.SkillTemplate.TryGet(comboSkillTypeID, out SkillTM comboSkillTM)) {
                    TDLog.Error($"加载连击技能失败 - TypeID {comboSkillTypeID} 不存在 ");
                    continue;
                }

                var comboSkillModel = new SkillModel();
                comboSkillModel.FromTM(comboSkillTM);
                comboSkillModel.SetID(idService.PickSkillID());
                comboSkillModel.SetOwner(owner);
                skillSlotCom.AddComboSkill(comboSkillModel);

            }
        }

        #endregion

        #region [Input]

        public void BackPlayerInput() {
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

            // - Skill Melee && HoldMelee
            if (inputGetter.GetDown(InputKeyCollection.MELEE)) {
                inputCom.SetInput_Skill__Melee(true);
            }

            // - Skill SpecMelee
            if (inputGetter.GetDown(InputKeyCollection.SPEC_MELEE)) {
                inputCom.SetInput_Skill_SpecMelee(true);
            }

            // - Skill BoomMelee
            if (inputGetter.GetDown(InputKeyCollection.BOOM_MELEE)) {
                inputCom.SetInput_Skill_BoomMelee(true);
            }

            // - Skill Infinity
            if (inputGetter.GetDown(InputKeyCollection.INFINITY)) {
                inputCom.SetInput_Skill_Infinity(true);
            }

            // - Skill Dash
            if (inputGetter.GetDown(InputKeyCollection.DASH)) {
                inputCom.SetInput_Skill_Dash(true);
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

        #endregion

        #region [Locomotion]

        public void Move(RoleEntity role) {
            role.Move();
        }

        public void SetRoleFaceDirX(RoleEntity role, sbyte dirX) {
            role.FaceTo(dirX);
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

        #region [Role Casting Skill]

        public bool TryCastSkillByInput(RoleEntity role) {
            var inputCom = role.InputCom;
            SkillType inputSkillType = inputCom.GetSkillType();
            if (inputSkillType == SkillType.None) {
                return false;
            }

            var weaponSlotCom = role.WeaponSlotCom;
            if (!weaponSlotCom.IsActive) {
                TDLog.Warning($"无法施放技能 - 武器未激活");
                return false;
            }

            var skillSlotCom = role.SkillSlotCom;
            if (!skillSlotCom.TryGetOriginalSkillByType(inputSkillType, out var originalSkill)) {
                TDLog.Error($"施放技能失败 - 不存在原始技能类型 {inputSkillType}");
                return false;
            }

            int inputSkillTypeID = originalSkill.TypeID;

            var fsm = role.FSMCom;
            if (fsm.State == RoleFSMState.Idle) {
                CastOriginalSkill(role, inputSkillTypeID);
                return true;
            }

            // 技能连招逻辑
            bool isCasting = fsm.State == RoleFSMState.Casting;
            if (isCasting) {
                var stateModel = fsm.CastingModel;
                var castingSkillTypeID = stateModel.castingSkillTypeID;
                SkillModel castingSkill;
                if (stateModel.IsCombo) {
                    skillSlotCom.TryGetComboSkill(castingSkillTypeID, out castingSkill);
                } else {
                    skillSlotCom.TryGetOriginalSkillByTypeID(castingSkillTypeID, out castingSkill);
                }

                if (CanCancelSkill(skillSlotCom, castingSkill, inputSkillTypeID, out var realSkillTypeID, out var isCombo)) {
                    castingSkill.Reset();
                    if (isCombo) {
                        CastComboSkill(role, realSkillTypeID);
                    } else {
                        CastOriginalSkill(role, realSkillTypeID);
                    }
                }
            }

            return true;
        }

        bool CanCancelSkill(SkillSlotComponent skillSlotCom, SkillModel castingSkill, int inputSkillTypeID, out int realSkillTypeID, out bool isCombo) {
            realSkillTypeID = inputSkillTypeID;
            isCombo = false;

            if (!castingSkill.TryGetCurrentFrame(out var frame)) {
                TDLog.Error($"技能未配置帧 - {castingSkill.TypeID} ");
                return false;
            }

            var allCancelModels = frame.allCancelModels;
            if (allCancelModels == null || allCancelModels.Length == 0) {
                return false;
            }

            for (int i = 0; i < allCancelModels.Length; i++) {
                SkillCancelModel cancel = allCancelModels[i];
                int cancelSkillTypeID = cancel.skillTypeID;

                if (cancel.isCombo) {
                    // - Combo Cancel
                    bool hasCombo = skillSlotCom.TryGetComboSkill(cancelSkillTypeID, out var comboSkill);
                    bool isComboInput = inputSkillTypeID == comboSkill.OriginalSkillTypeID;
                    if (hasCombo && isComboInput) {
                        realSkillTypeID = cancelSkillTypeID;    // 改变技能类型ID
                        isCombo = true;
                        return true;
                    }
                } else {
                    // - Normal Cancel
                    bool hasOriginal = skillSlotCom.TryGetOriginalSkillByTypeID(cancelSkillTypeID, out var originalSkill);
                    bool isOriginalInput = inputSkillTypeID == cancelSkillTypeID;
                    if (hasOriginal && isOriginalInput) {
                        isCombo = false;
                        return true;
                    }
                }
            }
            return false;
        }

        void CastOriginalSkill(RoleEntity role, int typeID) {
            var skillSlotCom = role.SkillSlotCom;
            if (!skillSlotCom.TryGetOriginalSkillByTypeID(typeID, out var skill)) {
                TDLog.Error($"施放原始技能失败:{typeID} ");
                return;
            }
            var fsm = role.FSMCom;
            fsm.EnterCasting(skill, false);
        }

        void CastComboSkill(RoleEntity role, int typeID) {
            var skillSlotCom = role.SkillSlotCom;
            if (!skillSlotCom.TryGetComboSkill(typeID, out var skill)) {
                TDLog.Error($"施放连击技能失败:{typeID} ");
                return;
            }
            var fsm = role.FSMCom;
            fsm.EnterCasting(skill, true);
        }

        #endregion

        #region [Role Hit]

        void BeHitBySkill(RoleEntity other, SkillModel skill) {

            var caster = skill.Owner;

            // Me Check
            if (caster == other) {
                return;
            }

            // Ally Check
            var casterIDCom = caster.IDCom;
            var otherIDCom = other.IDCom;
            if (casterIDCom.AllyType == otherIDCom.AllyType) {
                return;
            }

            // Damage Arbit: Prevent Multi Hit
            var damageArbitService = worldContext.DamageArbitService;
            if (damageArbitService.IsInArbit(skill.EntityType, skill.ID, otherIDCom.EntityType, otherIDCom.EntityID)) {
                return;
            }
            damageArbitService.TryAdd(skill.EntityType, skill.ID, otherIDCom.EntityType, otherIDCom.EntityID);

            DamagedBySkill(caster, skill, other);
            if (other.AttrCom.HP <= 0) {
                BeginDying(other);
            } else {
                FrameEffector(caster, skill, other);
            }

        }

        void DamagedBySkill(RoleEntity caster, SkillModel skill, RoleEntity other) {
            // Weapon Damage
            var curWeapon = caster.WeaponSlotCom.Weapon;
            other.HitBeHit(curWeapon.atk);
        }

        void FrameEffector(RoleEntity caster, SkillModel skill, RoleEntity other) {

        }

        #endregion

        #region [Role State]

        public void BeginDying(RoleEntity role) {
            var fsm = role.FSMCom;
            fsm.EnterDying(30);
        }

        public void Die(RoleEntity role) {
            role.Hide();
        }

        public bool CanEnterDying(RoleEntity role) {
            var attrCom = role.AttrCom;
            return attrCom.HP <= 0;
        }

        #endregion

        #region [Role Physics Event]

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

        void OnTriggerEnter_Skill(SkillModel skill, Collider2D other) {
            var go = other.gameObject;
            var otherRole = go.GetComponentInParent<RoleEntity>();
            if (otherRole != null) {
                BeHitBySkill(otherRole, skill);
            }
        }

        #endregion

    }

}
using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

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
            // Weapon Slot
            SetWeaponSlotComponent(role, weaponTypeID);

            // Skill Slot
            var skillSlotCom = role.SkillSlotCom;
            var curWeapon = role.WeaponSlotCom.Weapon;
            var skillTypeIDArray = new int[] { curWeapon.skillMeleeTypeID, curWeapon.skillHoldMeleeTypeID, curWeapon.skillSpecMeleeTypeID };
            var idArgs = role.IDCom.ToArgs();

            var rootDomain = worldContext.RootDomain;

            var skillDomain = rootDomain.SkillDomain;
            skillDomain.AddAllSkillToSlot_Origin(skillSlotCom, skillTypeIDArray, idArgs);
            skillDomain.AddAllSkillToSlot_Combo(skillSlotCom, idArgs);

            skillSlotCom.Foreach_Origin((skill) => {
                rootDomain.SetFather_CollisionTriggerModelArray(skill.CollisionTriggerArray, idArgs);
            });
            skillSlotCom.Foreach_Combo((skill) => {
                rootDomain.SetFather_CollisionTriggerModelArray(skill.CollisionTriggerArray, idArgs);
            });
        }

        public void SetWeaponSlotComponent(RoleEntity role, int weaponTypeID) {
            var weaponModel = SpawnWeaponModel(weaponTypeID);
            if (weaponModel == null) {
                TDLog.Error($"武器生成失败 - {weaponTypeID}");
                return;
            }

            var mod = weaponModel.Mod;
            mod.transform.SetParent(role.WeaponSlotCom.WeaponRoot, false);
            role.WeaponSlotCom.SetWeapon(weaponModel);
        }

        public WeaponEntity SpawnWeaponModel(int typeID) {
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

        public void FaceToChoosePoint(RoleEntity role) {
            var choosePoint = role.InputCom.ChoosePoint;
            if (choosePoint != Vector2.zero) {
                var rolePos = role.GetPos_LogicRoot();
                var xDiff = choosePoint.x - rolePos.x;
                var dirX = (sbyte)(xDiff > 0 ? 1 : xDiff == 0 ? 0 : -1);
                role.FaceTo(dirX);
            }
        }

        public void FaceToMoveDir(RoleEntity role) {
            var inputCom = role.InputCom;
            var x = inputCom.MoveAxis.x;
            var dirX = (sbyte)(x > 0 ? 1 : x == 0 ? 0 : -1);
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
            if (!skillSlotCom.TrgGet_OriginSkill_BySkillType(inputSkillType, out var originalSkill)) {
                TDLog.Error($"施放技能失败 - 不存在原始技能类型 {inputSkillType}");
                return false;
            }

            int originSkillTypeID = originalSkill.IDCom.TypeID;

            // 正常释放
            var fsm = role.FSMCom;
            if (fsm.State == RoleFSMState.Idle) {
                CastOriginalSkill(role, originSkillTypeID);
                return true;
            }

            // 连招
            if (fsm.State == RoleFSMState.Casting) {
                var stateModel = fsm.CastingModel;
                var castingSkillTypeID = stateModel.castingSkillTypeID;
                SkillEntity castingSkill;
                if (stateModel.IsCombo) {
                    _ = skillSlotCom.TryGet_Combo(castingSkillTypeID, out castingSkill);
                } else {
                    _ = skillSlotCom.TryGet_Origin(castingSkillTypeID, out castingSkill);
                }

                if (CanCancelSkill(skillSlotCom, castingSkill, originSkillTypeID, out var realSkillTypeID, out var cancelType)) {
                    castingSkill.Reset();
                    if (cancelType == SkillCancelType.Combo) CastComboSkill(role, realSkillTypeID);
                    else CastOriginalSkill(role, realSkillTypeID);
                    return true;
                }
            }

            return false;
        }

        bool CanCancelSkill(SkillSlotComponent skillSlotCom, SkillEntity castingSkill, int inputSkillTypeID, out int realSkillTypeID, out SkillCancelType cancelType) {
            // 默认赋值
            realSkillTypeID = inputSkillTypeID;

            bool isLink = false;
            int linkSkillTypeID = -1;
            // 检查是否为 非组合技连招
            castingSkill.Foreach_CancelModel_Link_InCurrentFrame((cancelModel) => {
                int skillTypeID = cancelModel.skillTypeID;
                if (!skillSlotCom.TryGet_Origin(skillTypeID, out _)) return;
                isLink = true;
                linkSkillTypeID = skillTypeID;
            });
            if (isLink) {
                realSkillTypeID = linkSkillTypeID;
                cancelType = SkillCancelType.Link;
                return true;
            }

            // 检查是否为 组合技
            bool isCombo = false;
            int comboSkillTypeID = -1;
            castingSkill.Foreach_CancelModel_Combo_InCurrentFrame((cancelModel) => {
                int skillTypeID = cancelModel.skillTypeID;
                if (!skillSlotCom.TryGet_Combo(skillTypeID, out _)) return;
                comboSkillTypeID = skillTypeID;
                isCombo = true;
            });
            if (isCombo) {
                realSkillTypeID = comboSkillTypeID;
                cancelType = SkillCancelType.Combo;
                return true;
            }

            cancelType = SkillCancelType.None;
            return false;
        }

        void CastOriginalSkill(RoleEntity role, int skillTypeID) {
            var fsmCom = role.FSMCom;
            fsmCom.EnterCasting(skillTypeID, false);
        }

        void CastComboSkill(RoleEntity role, int skillTypeID) {
            var fsmCom = role.FSMCom;
            fsmCom.EnterCasting(skillTypeID, true);
        }

        #endregion

        #region [Role Hit]

        void BeHitBySkill(RoleEntity other, SkillEntity skill) {

            // var caster = skill.Owner;

            // // Me Check
            // if (caster == other) {
            //     return;
            // }

            // // Ally Check
            // var casterIDCom = caster.IDCom;
            // var otherIDCom = other.IDCom;
            // if (casterIDCom.AllyType == otherIDCom.AllyType) {
            //     return;
            // }

            // // Damage Arbit: Prevent Multi Hit
            // var damageArbitService = worldContext.DamageArbitService;
            // if (damageArbitService.IsInArbit(skill.EntityType, skill.ID, otherIDCom.EntityType, otherIDCom.EntityID)) {
            //     return;
            // }
            // damageArbitService.TryAdd(skill.EntityType, skill.ID, otherIDCom.EntityType, otherIDCom.EntityID);

            // DamagedBySkill(caster, skill, other);
            // if (other.AttrCom.HP <= 0) {
            //     BeginDying(other);
            // } else {
            //     FrameEffector(caster, skill, other);
            // }

        }

        void DamagedBySkill(RoleEntity caster, SkillEntity skill, RoleEntity other) {
            // Weapon Damage
            var curWeapon = caster.WeaponSlotCom.Weapon;
            other.HitBeHit(curWeapon.atk);
        }

        void FrameEffector(RoleEntity caster, SkillEntity skill, RoleEntity other) {

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
            var attrCom = role.AttributeCom;
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

        void OnTriggerEnter_Skill(SkillEntity skill, Collider2D other) {
            var go = other.gameObject;
            var otherRole = go.GetComponentInParent<RoleEntity>();
            if (otherRole != null) {
                BeHitBySkill(otherRole, skill);
            }
        }

        #endregion

    }

}
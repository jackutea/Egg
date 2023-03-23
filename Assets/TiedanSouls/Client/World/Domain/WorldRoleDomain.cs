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

        /// <summary>
        /// 根据实体生成模型 生成角色
        /// </summary>
        public bool TrySpawnRole(int fromFieldTypeID, in EntitySpawnModel entitySpawnModel, out RoleEntity role) {
            var typeID = entitySpawnModel.typeID;
            var pos = entitySpawnModel.spawnPos;
            var allyType = entitySpawnModel.allyType;
            var controlType = entitySpawnModel.controlType;

            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateRoleEntity(typeID, out role)) {
                TDLog.Error($"创建角色失败! - {typeID}");
                return false;
            }

            BaseSetRole(role, typeID, pos, allyType, controlType);

            var idCom = role.IDCom;
            idCom.SetFromFieldTypeID(fromFieldTypeID);
            role.SetIsBoss(entitySpawnModel.isBoss);

            var repo = worldContext.RoleRepo;
            if (idCom.ControlType == ControlType.Player) {
                repo.Set_Player(role);
            } else if (idCom.ControlType == ControlType.AI) {
                var ai = role.AIStrategy;
                ai.Activate();
                repo.Add_ToAI(role);
            }
            
            return true;
        }

        /// <summary>
        /// 根据实体召唤模型 召唤角色
        /// </summary>
        public bool TrySummonRole(Vector3 summonPos, Quaternion summonRot, in IDArgs summoner, in EntitySummonModel entitySummonModel, out RoleEntity role) {
            var typeID = entitySummonModel.typeID;
            var controlType = entitySummonModel.controlType;
            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateRoleEntity(typeID, out role)) {
                TDLog.Error($"创建角色失败! - {typeID}");
                return false;
            }

            BaseSetRole(role, typeID, summonPos, summoner.allyType, controlType);

            var idCom = role.IDCom;
            idCom.SetFather(summoner);

            var repo = worldContext.RoleRepo;
            if (idCom.ControlType == ControlType.Player) {
                repo.Set_Player(role);
            } else if (idCom.ControlType == ControlType.AI) {
                var ai = role.AIStrategy;
                ai.Activate();
                repo.Add_ToAI(role);
            }

            return true;
        }

        /// <summary>
        /// 设置角色基础信息
        /// </summary>
        void BaseSetRole(RoleEntity role, int typeID, Vector3 pos, AllyType allyType, ControlType controlType) {
            // Pos
            role.SetPos_Logic(pos);
            role.Renderer_Sync();

            // ID
            var idService = worldContext.IDService;
            int id = idService.PickRoleID();
            var idCom = role.IDCom;
            idCom.SetEntityID(id);
            idCom.SetControlType(controlType);
            idCom.SetAllyType(allyType);

            // Physics
            role.FootTriggerEnterAction += OnFootTriggerEnter;
            role.FootTriggerExit += OnFootTriggerExit;

            // Collider Model
            var colliderModel = role.LogicRoot.gameObject.AddComponent<ColliderModel>();
            colliderModel.SetFather(idCom.ToArgs());

            // AI
            if (controlType == ControlType.AI) {
                var factory = worldContext.WorldFactory;
                var ai = factory.CreateAIStrategy(role, typeID);
                role.SetAIStrategy(ai);
            }

            // HUD Show
            if (idCom.AllyType == AllyType.Two) role.HudSlotCom.HpBarHUD.SetColor(Color.red);
            else if (idCom.AllyType == AllyType.Neutral) role.HudSlotCom.HpBarHUD.SetColor(Color.yellow);
        }

        #region [玩家角色 拾取武器 -> 初始化武器组件 -> 添加对应技能]

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

            var rootDomain = worldContext.RootDomain;

            var skillDomain = rootDomain.SkillDomain;
            var roleIDArgs = role.IDCom.ToArgs();
            skillDomain.AddAllSkillToSlot_Origin(skillSlotCom, skillTypeIDArray, roleIDArgs);
            skillDomain.AddAllSkillToSlot_Combo(skillSlotCom, roleIDArgs);
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

        #region [玩家角色输入]

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
            moveAxis.Normalize();
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
                var chosenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                inputCom.SetInput_Basic_ChosenPoint(chosenPoint);
            }
        }

        #endregion

        #region [Locomotion]

        public void MoveByInput(RoleEntity role) {
            role.MoveByInput();
        }

        public void FaceTo_Horizontal(RoleEntity role, Vector2 point) {
            if (point != Vector2.zero) {
                var rolePos = role.GetPos_LogicRoot();
                var xDiff = point.x - rolePos.x;
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

        public void JumpByInput(RoleEntity role) {
            role.JumpByInput();
        }

        public void CrossDown(RoleEntity role) {
            role.TryCrossDown();
        }

        public void Fall(RoleEntity role, float dt) {
            if (role.IsGrounded) {
                return;
            }

            role.Falling(dt);
        }

        #endregion

        #region [角色释放技能]

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
            if (fsm.StateFlag == StateFlag.Idle) {
                CastOriginalSkill(role, originSkillTypeID);
                return true;
            }

            // 连招
            if (fsm.StateFlag.Contains(StateFlag.Cast)) {
                var stateModel = fsm.CastingModel;
                var castingSkillTypeID = stateModel.CastingSkillTypeID;
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
            realSkillTypeID = inputSkillTypeID;

            if (castingSkill.OriginalSkillTypeID == inputSkillTypeID) {
                // 检查是否为 组合技
                bool isComboSkill = false;
                int comboSkillTypeID = -1;
                castingSkill.Foreach_CancelModel_Combo_InCurrentFrame((cancelModel) => {
                    int skillTypeID = cancelModel.skillTypeID;
                    if (!skillSlotCom.TryGet_Combo(skillTypeID, out _)) return;
                    comboSkillTypeID = skillTypeID;
                    isComboSkill = true;
                });
                if (isComboSkill) {
                    realSkillTypeID = comboSkillTypeID;
                    cancelType = SkillCancelType.Combo;
                    return true;
                }
            } else {
                bool isLinkedSkill = false;
                int linkedSkillTypeID = -1;
                // 检查是否为 非组合技连招
                castingSkill.Foreach_CancelModel_Linked_InCurrentFrame((cancelModel) => {
                    int skillTypeID = cancelModel.skillTypeID;
                    if (!skillSlotCom.TryGet_Origin(skillTypeID, out var linkedSkill)) return;
                    linkedSkillTypeID = skillTypeID;
                    isLinkedSkill = true;
                });
                if (isLinkedSkill) {
                    realSkillTypeID = linkedSkillTypeID;
                    cancelType = SkillCancelType.Link;
                    return true;
                }
            }

            cancelType = SkillCancelType.None;
            return false;
        }

        void CastOriginalSkill(RoleEntity role, int skillTypeID) {
            var fsmCom = role.FSMCom;
            fsmCom.AddCast(skillTypeID, false, role.InputCom.ChosenPoint);
        }

        void CastComboSkill(RoleEntity role, int skillTypeID) {
            var fsmCom = role.FSMCom;
            fsmCom.AddCast(skillTypeID, true, role.InputCom.ChosenPoint);
        }

        #endregion

        #region [角色状态]

        public void Role_PrepareToDie(RoleEntity role) {
            var roleRepo = worldContext.RoleRepo;
            var fsm = role.FSMCom;
            fsm.AddDying(30);
        }

        public void Role_Die(RoleEntity role) {
            TDLog.Log($"角色死亡 - {role.IDCom.TypeID}");
            role.FSMCom.SetIsExiting(true);
            role.AttributeCom.ClearHP();
            role.Hide();
        }

        public bool IsRoleDead(RoleEntity role) {
            var attrCom = role.AttributeCom;
            return attrCom.HP <= 0;
        }

        #endregion

        #region [角色物理事件]

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

        #endregion

    }

}
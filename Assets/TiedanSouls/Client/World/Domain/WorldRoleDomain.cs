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
            var campType = entitySpawnModel.campType;
            var controlType = entitySpawnModel.controlType;

            var factory = worldContext.Factory;
            if (!factory.TryCreateRoleEntity(typeID, out role)) {
                TDLog.Error($"创建角色失败! - {typeID}");
                return false;
            }

            BaseSetRole(role, typeID, pos, Quaternion.identity, campType, controlType);

            role.SetFromFieldTypeID(fromFieldTypeID);
            role.SetIsBoss(entitySpawnModel.isBoss);

            var repo = worldContext.RoleRepo;
            var idCom = role.IDCom;
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
        public bool TrySummonRole(Vector3 summonPos, Quaternion summonRot, in EntityIDArgs summoner, in RoleSummonModel roleSummonModel, out RoleEntity role) {
            var typeID = roleSummonModel.typeID;
            var controlType = roleSummonModel.controlType;
            var factory = worldContext.Factory;
            if (!factory.TryCreateRoleEntity(typeID, out role)) {
                TDLog.Error($"创建角色失败! - {typeID}");
                return false;
            }

            BaseSetRole(role, typeID, summonPos, summonRot, summoner.campType, controlType);

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

        public void ModifyRole(RoleEntity role, in RoleEffectorModel roleEffectorModel) {
            var buffDomain = worldContext.RootDomain.BuffDomain;
            var roleAttributeSelectorModel = roleEffectorModel.roleAttributeSelectorModel;
            var roleAttributeModifyModel = roleEffectorModel.roleAttributeModifyModel;
            var curFieldTypeID = worldContext.StateEntity.CurFieldTypeID;
            var roleRepo = worldContext.RoleRepo;

            roleRepo.Foreach_All((role) => {
                var attrCom = role.AttributeCom;
                if (!attrCom.IsMatch(roleAttributeSelectorModel)) return;
                buffDomain.TryEffectRoleAttribute(role.AttributeCom, roleAttributeModifyModel, 1);
            });
        }

        /// <summary>
        /// 设置角色基础信息
        /// </summary>
        void BaseSetRole(RoleEntity role, int typeID, Vector3 pos, Quaternion rot, CampType campType, ControlType controlType) {
            // Pos
            role.SetPos(pos);
            role.SetRotation(rot);
            Renderer_Sync(role);

            // ID
            var idService = worldContext.IDService;
            var id = idService.PickRoleID();
            var idCom = role.IDCom;
            idCom.SetEntityID(id);
            idCom.SetControlType(controlType);
            idCom.SetAllyType(campType);

            // Collider Model
            var entityCollider = role.LogicRoot.gameObject.AddComponent<EntityCollider>();
            var rootDomain = worldContext.RootDomain;
            rootDomain.SetEntityColliderFather(entityCollider, idCom.ToArgs());

            // AI
            if (controlType == ControlType.AI) {
                var factory = worldContext.Factory;
                var ai = factory.CreateAIStrategy(role, typeID);
                role.SetAIStrategy(ai);
            }

            // HUD Show
            if (idCom.AllyStatus == CampType.Two) role.HudSlotCom.HPBarHUD.SetColor(Color.red);
            else if (idCom.AllyStatus == CampType.Neutral) role.HudSlotCom.HPBarHUD.SetColor(Color.yellow);
        }

        #region [玩家角色 拾取武器 -> 初始化武器组件 -> 添加对应技能]

        public bool TryPickUpSomethingFromField(RoleEntity role) {
            var repo = worldContext.ItemRepo;
            var fieldTypeID = worldContext.StateEntity.CurFieldTypeID;
            if (!repo.TryGetOneItemFromField(fieldTypeID, role.RootPos, 1, out var item)) {
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
            var weaponEntity = SpawnWeapon(weaponTypeID);
            if (weaponEntity == null) {
                TDLog.Error($"武器生成失败 - {weaponTypeID}");
                return;
            }

            var mod = weaponEntity.Mod;
            mod.transform.SetParent(role.WeaponSlotCom.WeaponRoot, false);
            role.WeaponSlotCom.SetWeapon(weaponEntity);

            // Skill Slot
            var skillSlotCom = role.SkillSlotCom;
            var curWeapon = role.WeaponSlotCom.Weapon;
            var skillTypeIDArray = new int[] { curWeapon.SkillMeleeTypeID, curWeapon.SkillHoldMeleeTypeID, curWeapon.SkillSpecMeleeTypeID };

            var skillDomain = worldContext.RootDomain.SkillDomain;
            var roleIDArgs = role.IDCom.ToArgs();
            skillDomain.AddAllSkillToSlot_Origin(skillSlotCom, skillTypeIDArray, roleIDArgs);
            skillDomain.AddAllSkillToSlot_Combo(skillSlotCom, roleIDArgs);
        }

        public WeaponEntity SpawnWeapon(int typeID) {
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
            has = assetCore.WeaponModAsset.TryGet(weaponTM.meshName, out GameObject weaponModPrefab);
            if (!has) {
                TDLog.Error("Failed to get weapon mod: " + weaponTM.meshName);
                return null;
            }

            var idCom = weapon.IDCom;
            idCom.SetTypeID(weaponTM.typeID);

            weapon.SetWeaponType(weaponTM.weaponType);
            weapon.SetSkillMeleeTypeID(weaponTM.skillMeleeTypeID);
            weapon.SetSkillHoldMeleeTypeID(weaponTM.skillHoldMeleeTypeID);
            weapon.SetSkillSpecMeleeTypeID(weaponTM.skillSpecMeleeTypeID);

            var go = GameObject.Instantiate(weaponModPrefab);
            weapon.SetMod(go);

            return weapon;
        }

        #endregion

        /// <summary>
        /// 玩家角色输入
        /// </summary>
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
            }
            if (inputGetter.GetPressing(InputKeyCollection.MOVE_DOWN)) {
                moveAxis.y = -1;
            } else if (inputGetter.GetPressing(InputKeyCollection.MOVE_UP)) {
                moveAxis.y = 1;
            }
            bool hasInputMove = moveAxis != Vector2.zero;
            if (hasInputMove) {
                moveAxis.Normalize();
                inputCom.SetMoveAxis(moveAxis);
                inputCom.SetHasMoveOpt(true);
            } else {
                bool hasLooseMove = inputGetter.GetUp(InputKeyCollection.MOVE_LEFT)
                                    || inputGetter.GetUp(InputKeyCollection.MOVE_RIGHT)
                                    || inputGetter.GetUp(InputKeyCollection.MOVE_UP)
                                    || inputGetter.GetUp(InputKeyCollection.MOVE_DOWN);
                if (hasLooseMove) {
                    inputCom.SetHasMoveOpt(true);
                    inputCom.SetMoveAxis(Vector2.zero);
                }
            }

            // - Jump
            if (inputGetter.GetDown(InputKeyCollection.JUMP)) {
                inputCom.SetInputJump(true);
            }

            // - Normal SKill
            if (inputGetter.GetDown(InputKeyCollection.NORMAL_SKILL)) {
                inputCom.SetInputSkillMelee(true);
            }

            // - Special Skill
            if (inputGetter.GetDown(InputKeyCollection.SPECTIAL_SKILL)) {
                inputCom.SetInputSpecialSkill(true);
            }

            // - Ultimate Skill
            if (inputGetter.GetDown(InputKeyCollection.ULTIMATE_SKILL)) {
                inputCom.SetInputUltimateSkill(true);
            }

            // - Dash Skill
            if (inputGetter.GetDown(InputKeyCollection.DASH_Skill)) {
                inputCom.SetInputDashSkill(true);
            }

            // - Pick
            if (inputGetter.GetDown(InputKeyCollection.PICK)) {
                inputCom.SetInputPick(true);
            }

            // - Choose Point
            if (inputGetter.GetDown(InputKeyCollection.CHOOSE_POINT)) {
                var chosenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                inputCom.SetChosenPoint(chosenPoint);
            }
        }

        public void FaceToHorizontalPoint(RoleEntity role, Vector2 point) {
            if (point != Vector2.zero) {
                var rolePos = role.RootPos;
                var xDiff = point.x - rolePos.x;
                role.HorizontalFaceTo(xDiff);
            }
        }

        public void TryCrossDownPlatformByInput(RoleEntity role) {
            var inputCom = role.InputCom;
            if (!inputCom.HasMoveOpt) return;

            var wantCrossDown = inputCom.MoveAxis.y < 0;
            if (wantCrossDown) {
                role.SetTrigger(true);
                var fsmCom = role.FSMCom;
                fsmCom.RemovePositionStatus_StandInCrossPlatform();
                fsmCom.Enter_JumpingDown();
            }
        }

        public void Dash(RoleEntity role, Vector2 dir, Vector2 force) {
            var moveCom = role.MoveCom;
            moveCom.Dash(dir, force);
        }

        /// <summary>
        /// 角色释放技能
        /// </summary>
        public bool TryCastSkillByInput(RoleEntity role) {
            var inputCom = role.InputCom;
            SkillType inputSkillType = inputCom.GetSkillType();
            return TryCastSkill(role, inputSkillType);
        }

        public bool TryCastSkill(RoleEntity role, SkillType inputSkillType) {
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
            var roleFSMDomain = this.worldContext.RootDomain.RoleFSMDomain;

            // 正常释放
            var fsmCom = role.FSMCom;
            if (fsmCom.FSMState == RoleFSMState.Idle
            || fsmCom.FSMState == RoleFSMState.Moving
            || fsmCom.FSMState == RoleFSMState.JumpingUp) {
                roleFSMDomain.Enter_Casting(role, originalSkill, false);
                return true;
            }

            // 连招
            if (fsmCom.FSMState == RoleFSMState.Casting) {
                var stateModel = fsmCom.CastingStateModel;
                var castingSkill = stateModel.CastingSkill;

                if (CanCancelSkill(skillSlotCom, castingSkill, originSkillTypeID, out var realSkillTypeID, out var cancelType)) {
                    castingSkill.Reset();
                    if (cancelType == SkillCancelType.Combo) roleFSMDomain.Enter_Casting(role, realSkillTypeID, true);
                    else roleFSMDomain.Enter_Casting(role, realSkillTypeID, false);
                    return true;
                }
            }

            return false;
        }

        bool CanCancelSkill(SkillSlotComponent skillSlotCom, SkillEntity castingSkill, int inputSkillTypeID, out SkillEntity realSkill, out SkillCancelType cancelType) {
            realSkill = null;

            if (castingSkill.OriginalSkillTypeID == inputSkillTypeID) {
                // 组合技
                bool isComboSkill = false;
                SkillEntity comboSkill = null;
                castingSkill.Foreach_CancelModel_Combo((cancelModel) => {
                    int skillTypeID = cancelModel.skillTypeID;
                    if (!skillSlotCom.TryGet_Combo(skillTypeID, out comboSkill)) return;
                    isComboSkill = true;
                });
                if (isComboSkill) {
                    realSkill = comboSkill;
                    cancelType = SkillCancelType.Combo;
                    return true;
                }
            } else {
                // 连招技
                bool isLinkedSkill = false;
                SkillEntity linkedSkill = null;
                castingSkill.Foreach_CancelModel_Linked((cancelModel) => {
                    int skillTypeID = cancelModel.skillTypeID;
                    if (!skillSlotCom.TryGet_Origin(skillTypeID, out linkedSkill)) return;
                    isLinkedSkill = true;
                });
                if (isLinkedSkill) {
                    realSkill = linkedSkill;
                    cancelType = SkillCancelType.Link;
                    return true;
                }
            }

            cancelType = SkillCancelType.None;
            return false;
        }

        /// <summary>
        /// 角色受击的统一处理方式
        /// </summary>
        public void HandleBeHit(int hitFrame, Vector2 beHitDir, RoleEntity role, in EntityIDArgs hitter, in EntityColliderTriggerModel collisionTriggerModel) {
            var rootDomain = worldContext.RootDomain;
            var roleFSMDomain = rootDomain.RoleFSMDomain;
            var roleDomain = rootDomain.RoleDomain;
            var roleEffectorDomain = rootDomain.RoleEffectorDomain;
            var buffDomain = rootDomain.BuffDomain;

            // 受击
            var beHitModel = collisionTriggerModel.beHitModel;
            role.FSMCom.Enter_BeHit(beHitDir, beHitModel);

            var ctrlEffectSlotCom = role.CtrlEffectSlotCom;

            // 受击附加 角色控制效果
            var roleCtrlEffectModelArray = collisionTriggerModel.roleCtrlEffectModelArray;
            var len = roleCtrlEffectModelArray?.Length;
            for (int i = 0; i < len; i++) {
                var roleCtrlEffectModel = roleCtrlEffectModelArray[i];
                ctrlEffectSlotCom.AddCtrlEffect(roleCtrlEffectModel);
            }

            // 受击附加 角色效果器
            var targetRoleEffectorTypeIDArray = collisionTriggerModel.targetRoleEffectorTypeIDArray;
            len = targetRoleEffectorTypeIDArray.Length;
            for (int i = 0; i < len; i++) {
                var roleEffectorTypeID = targetRoleEffectorTypeIDArray[i];
                if (!roleEffectorDomain.TrySpawnRoleEffectorModel(roleEffectorTypeID, out var roleEffectorModel)) continue;
                buffDomain.TryEffectRoleAttribute(role.AttributeCom, roleEffectorModel.roleAttributeModifyModel, 1);
            }

            // 伤害 仲裁
            var damageArbitService = worldContext.DamageArbitService;
            var damageModel = collisionTriggerModel.damageModel;
            var baseDamage = damageModel.GetDamage(hitFrame);
            var damageType = damageModel.damageType;
            float realDamage = baseDamage;

            if (rootDomain.TryGetRoleFromIDArgs(hitter, out var hitterRole)) {
                realDamage = damageArbitService.ArbitrateDamage(damageType, baseDamage, hitterRole.AttributeCom, role.AttributeCom);
            }

            // 伤害结算
            roleDomain.ReduceHP(role, realDamage);
            damageArbitService.Add(damageModel.damageType, realDamage, role.IDCom.ToArgs(), hitter);

            // HUD伤害浮字
            var damageFloatTextHUD = role.HudSlotCom.DamageFloatTextHUD;
            damageFloatTextHUD.ShowDamageFloatText(damageType, realDamage, 0.5f);
        }

        public void TearDownRole(RoleEntity role) {
            TDLog.Log($"角色 TearDown - {role.IDCom.TypeID}");
            role.FSMCom.ResetAll();
            role.AttributeCom.ClearHP();
            role.Coll_LogicRoot.enabled = false;
            role.Hide();
        }

        public bool IsRoleDead(RoleEntity role) {
            var attrCom = role.AttributeCom;
            return attrCom.HP <= 0;
        }

        public void Renderer_Sync(RoleEntity role) {
            var rootPos = role.RootPos;
            var rootRotation = role.RootRotation;
            var headPos = role.GetHeadPos();

            role.RendererCom.SetPos(rootPos);
            role.RendererCom.SetRotation(rootRotation);

            role.HudSlotCom.SetPos(headPos);
        }

        public void Show(RoleEntity role) {
            role.LogicRoot.gameObject.SetActive(true);
            role.RendererCom.Show();
            role.HudSlotCom.ShowHUD();
            TDLog.Log($"显示角色: {role.IDCom.EntityName} ");
        }

        public void RecycleFieldRoles(int fieldTypeID) {
            var roleRepo = worldContext.RoleRepo;
            roleRepo.Foreach_ByFieldTypeID(fieldTypeID, (role) => {
                role.Hide();
            });
        }

        public void ResetAllAIs(int fieldTypeID, bool isShow) {
            var roleRepo = worldContext.RoleRepo;
            roleRepo.Foreach_ByFieldTypeID(fieldTypeID, (role) => {
                role.Reset();
                if (isShow) Show(role);
            });
        }

        #region [Physics Event Handle]

        void OnCollisionEnterField(RoleEntity role, Collision2D collision2D) {

        }

        void OnCollisionLeaveField(RoleEntity role, Collision2D collision2D) {
            var fsmCom = role.FSMCom;
            fsmCom.RemovePositionStatus_OnGround();
        }

        void OnCollisionEnterCrossPlatform(RoleEntity role, Collision2D collision2D) {
            var normal = collision2D.contacts[0].normal;
            var isStand = normal.y > 0;

            if (isStand) {
                var moveCom = role.MoveCom;
                var rb = moveCom.RB;
                var velo = rb.velocity;
                velo.y = 0;
                moveCom.SetVelocity(velo);

                var fsmCom = role.FSMCom;
                fsmCom.AddPositionStatus_StandInCrossPlatform();
                fsmCom.Enter_Idle();
            }
        }

        void OnCollisionLeavePlatform(RoleEntity role, Collision2D collision2D) {
            var fsmCom = role.FSMCom;
            fsmCom.RemovePositionStatus_StandInCrossPlatform();
        }

        void OnTriggerEnterField(RoleEntity role, Collider2D collider2D) {
        }

        void OnTriggerLeaveField(RoleEntity role, Collider2D collider2D) {
        }

        void OnTriggerEnterCrossPlatform(RoleEntity role, Collider2D collider2D) {
        }

        void OnTriggerLeaveCrossPlatform(RoleEntity role, Collider2D collider2D) {
            role.SetTrigger(false);
            var fsmCom = role.FSMCom;
            fsmCom.Enter_Falling();
        }

        #endregion

        #region [Attribute]

        public float ReduceHP(RoleEntity role, float damage) {
            var attributeCom = role.AttributeCom;
            var decrease = attributeCom.ReduceHP(damage);
            var hudSlotCom = role.HudSlotCom;
            var hpBar = hudSlotCom.HPBarHUD;
            hpBar.SetGP(attributeCom.GP);
            hpBar.SetHP(attributeCom.HP);
            hpBar.SetHPMax(attributeCom.HPMax);

            TDLog.Log($"{role.IDCom.EntityName} 受到伤害 {damage} HP减少: {decrease}");
            return decrease;
        }

        public float ReduceHP_Percentage(RoleEntity role, float percentage) {
            var attributeCom = role.AttributeCom;
            var hudSlotCom = role.HudSlotCom;

            var curHP = attributeCom.HP;
            var decrease = curHP * percentage;
            decrease = attributeCom.ReduceHP(decrease);

            TDLog.Log($"{role.IDCom.EntityName} 受到百分比伤害 百分比: {percentage} HP减少: {decrease}");
            return decrease;
        }

        #endregion

    }

}
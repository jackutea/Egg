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

            var factory = worldContext.Factory;
            if (!factory.TryCreateRoleEntity(typeID, out role)) {
                TDLog.Error($"创建角色失败! - {typeID}");
                return false;
            }

            BaseSetRole(role, typeID, pos, allyType, controlType);

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
        public bool TrySummon(Vector3 summonPos, Quaternion summonRot, in EntityIDArgs summoner, in EntitySummonModel entitySummonModel, out RoleEntity role) {
            var typeID = entitySummonModel.typeID;
            var controlType = entitySummonModel.controlType;
            var factory = worldContext.Factory;
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
            role.SetLogicPos(pos);
            Renderer_Sync(role);

            // ID
            var idService = worldContext.IDService;
            var id = idService.PickRoleID();
            var idCom = role.IDCom;
            idCom.SetEntityID(id);
            idCom.SetControlType(controlType);
            idCom.SetAllyType(allyType);

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
            if (idCom.AllyStatus == AllyType.Two) role.HudSlotCom.HpBarHUD.SetColor(Color.red);
            else if (idCom.AllyStatus == AllyType.Neutral) role.HudSlotCom.HpBarHUD.SetColor(Color.yellow);
        }

        #region [玩家角色 拾取武器 -> 初始化武器组件 -> 添加对应技能]

        public bool TryPickUpSomethingFromField(RoleEntity role) {
            var repo = worldContext.ItemRepo;
            var fieldTypeID = worldContext.StateEntity.CurFieldTypeID;
            if (!repo.TryGetOneItemFromField(fieldTypeID, role.LogicRootPos, 1, out var item)) {
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
            var skillTypeIDArray = new int[] { curWeapon.SkillMeleeTypeID, curWeapon.SkillHoldMeleeTypeID, curWeapon.SkillSpecMeleeTypeID };

            var rootDomain = worldContext.RootDomain;

            var skillDomain = rootDomain.SkillDomain;
            var roleIDArgs = role.IDCom.ToArgs();
            skillDomain.AddAllSkillToSlot_Origin(skillSlotCom, skillTypeIDArray, roleIDArgs);
            skillDomain.AddAllSkillToSlot_Combo(skillSlotCom, roleIDArgs);
        }

        public void SetWeaponSlotComponent(RoleEntity role, int weaponTypeID) {
            var weaponModel = SpawnWeapon(weaponTypeID);
            if (weaponModel == null) {
                TDLog.Error($"武器生成失败 - {weaponTypeID}");
                return;
            }

            var mod = weaponModel.Mod;
            mod.transform.SetParent(role.WeaponSlotCom.WeaponRoot, false);
            role.WeaponSlotCom.SetWeapon(weaponModel);
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
            }
            if (inputGetter.GetPressing(InputKeyCollection.MOVE_DOWN)) {
                moveAxis.y = -1;
            } else if (inputGetter.GetPressing(InputKeyCollection.MOVE_UP)) {
                moveAxis.y = 1;
            }
            bool hasPressMove = moveAxis != Vector2.zero;
            if (hasPressMove) {
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
                inputCom.SetPressJump(true);
            }

            // - Skill Melee && HoldMelee
            if (inputGetter.GetDown(InputKeyCollection.MELEE)) {
                inputCom.SetPressSkillMelee(true);
            }

            // - Skill SpecMelee
            if (inputGetter.GetDown(InputKeyCollection.SPEC_MELEE)) {
                inputCom.SetPressSkillSpecMelee(true);
            }

            // - Skill BoomMelee
            if (inputGetter.GetDown(InputKeyCollection.BOOM_MELEE)) {
                inputCom.SetPressSkillBoomMelee(true);
            }

            // - Skill Infinity
            if (inputGetter.GetDown(InputKeyCollection.INFINITY)) {
                inputCom.SetPressSkillInfinity(true);
            }

            // - Skill Dash
            if (inputGetter.GetDown(InputKeyCollection.DASH)) {
                inputCom.SetPressSkillDash(true);
            }

            // - Pick
            if (inputGetter.GetDown(InputKeyCollection.PICK)) {
                inputCom.SetPressPick(true);
            }

            // - Choose Point
            if (inputGetter.GetDown(InputKeyCollection.CHOOSE_POINT)) {
                var chosenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                inputCom.SetChosenPoint(chosenPoint);
            }
        }

        #endregion

        #region [Locomotion]

        public void StopMove(RoleEntity role) {
            var moveCom = role.MoveCom;
            moveCom.Stop();
        }

        public void FaceTo_Horizontal(RoleEntity role, Vector2 point) {
            if (point != Vector2.zero) {
                var rolePos = role.LogicRootPos;
                var xDiff = point.x - rolePos.x;
                role.SetLogicFaceTo(xDiff);
            }
        }

        public void FaceToMoveDir(RoleEntity role) {
            var inputCom = role.InputCom;
            var x = inputCom.MoveAxis.x;
            role.SetLogicFaceTo(x);
        }

        public void Fall(RoleEntity role, float dt) {
            var attributeCom = role.AttributeCom;
            var fallSpeed = attributeCom.FallSpeed;
            var fallSpeedMax = attributeCom.FallSpeedMax;

            var moveCom = role.MoveCom;
            var vel = moveCom.Velocity;
            vel.y = Mathf.Max(vel.y - fallSpeed * dt, -fallSpeedMax);
            moveCom.SetVerticalVelocity(vel);
        }

        public void TryMoveByInput(RoleEntity role) {
            var inputCom = role.InputCom;
            if (!inputCom.HasMoveOpt) return;

            Vector2 moveAxis = inputCom.MoveAxis;
            var moveCom = role.MoveCom;
            var attributeCom = role.AttributeCom;
            moveCom.MoveHorizontal(moveAxis.x, attributeCom.MoveSpeed);
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

        public void JumpByInput(RoleEntity role) {
            var inputCom = role.InputCom;
            if (!inputCom.PressJump) return;

            var moveCom = role.MoveCom;
            var attributeCom = role.AttributeCom;
            var rb = moveCom.RB;
            var velo = rb.velocity;
            var jumpSpeed = attributeCom.JumpSpeed;
            velo.y = jumpSpeed;
            moveCom.SetVelocity(velo);

            var fsm = role.FSMCom;
            fsm.Enter_JumpingUp();
        }

        public void Dash(RoleEntity role, Vector2 dir, Vector2 force) {
            var moveCom = role.MoveCom;
            moveCom.Dash(dir, force);
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

        #endregion

        #region [Attribute]

        public float ReduceHP(RoleEntity role, float damage) {
            var attributeCom = role.AttributeCom;
            var hudSlotCom = role.HudSlotCom;

            var decrease = attributeCom.ReduceHP(damage);
            hudSlotCom.HpBarHUD.SetHpBar(attributeCom.HP, attributeCom.HPMax);

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
            if (fsm.ActionState == RoleActionState.Idle
            || fsm.ActionState == RoleActionState.Moving
            || fsm.ActionState == RoleActionState.JumpingUp) {
                CastOriginalSkill(role, originSkillTypeID);
                return true;
            }

            // 连招
            if (fsm.ActionState == RoleActionState.Casting) {
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
            fsmCom.Enter_Cast(skillTypeID, false, role.InputCom.ChosenPoint);
        }

        void CastComboSkill(RoleEntity role, int skillTypeID) {
            var fsmCom = role.FSMCom;
            fsmCom.Enter_Cast(skillTypeID, true, role.InputCom.ChosenPoint);
        }

        #endregion

        #region [角色受击处理]

        /// <summary>
        /// 角色受击的统一处理方式
        /// </summary>
        public void HandleBeHit(int hitFrame, Vector2 beHitDir, RoleEntity role, in EntityIDArgs hitter, in EntityColliderTriggerModel collisionTriggerModel) {
            var roleFSMDomain = worldContext.RootDomain.RoleFSMDomain;
            var roleDomain = worldContext.RootDomain.RoleDomain;

            // 击退
            roleFSMDomain.AddCtrlStatus_KnockBack(role, beHitDir, collisionTriggerModel.knockBackModel);
            // 击飞
            roleFSMDomain.AddCtrlStatus_KnockUp(role, beHitDir, collisionTriggerModel.knockUpModel);

            // 伤害 仲裁
            var damageArbitService = worldContext.DamageArbitService;
            var damageModel = collisionTriggerModel.damageModel;
            var baseDamage = damageModel.GetDamage(hitFrame);
            var damageType = damageModel.damageType;
            float realDamage = baseDamage;

            RoleEntity roleFather = null;
            var rootDomain = worldContext.RootDomain;
            if (rootDomain.TryFindRoleFather(hitter, ref roleFather)) {
                realDamage = damageArbitService.ArbitrateDamage(damageType, baseDamage, roleFather.AttributeCom, role.AttributeCom);
            }

            // 伤害结算
            roleDomain.ReduceHP(role, realDamage);
            damageArbitService.Add(damageModel.damageType, realDamage, role.IDCom.ToArgs(), hitter);
        }

        #endregion

        #region [角色 TearDown 相关]

        public void TearDownRole(RoleEntity role) {
            TDLog.Log($"角色 TearDown - {role.IDCom.TypeID}");
            role.FSMCom.ResetAll();
            role.AttributeCom.ClearHP();
            Hide(role);
            DeactivateCollider(role);
        }

        public bool IsRoleDead(RoleEntity role) {
            var attrCom = role.AttributeCom;
            return attrCom.HP <= 0;
        }

        #endregion

        #region [表现层]

        public void Renderer_Sync(RoleEntity role) {
            var logicRoot = role.LogicRoot;
            var rendererRoot = role.RendererRoot;
            var weaponRoot = role.WeaponRoot;

            var logicPos = logicRoot.position;
            rendererRoot.position = logicPos;
            weaponRoot.position = logicPos;

            var rendererModCom = role.RendererModCom;
            rendererModCom.Mod.transform.rotation = logicRoot.rotation;
            weaponRoot.rotation = logicRoot.rotation;
        }

        public void Renderer_Easing(RoleEntity role, float dt) {
            var logicRoot = role.LogicRoot;
            var rendererRoot = role.RendererRoot;
            var weaponRoot = role.WeaponRoot;
            var rendererModCom = role.RendererModCom;

            var lerpPos = Vector3.Lerp(rendererRoot.position, logicRoot.position, dt * 30);
            rendererRoot.position = lerpPos;
            weaponRoot.position = lerpPos;

            rendererModCom.Mod.transform.rotation = logicRoot.rotation;
            weaponRoot.rotation = logicRoot.rotation;
        }

        #endregion

        #region [显示 & 隐藏]

        public void Show(RoleEntity role) {
            role.LogicRoot.gameObject.SetActive(true);
            role.RendererRoot.gameObject.SetActive(true);
            TDLog.Log($"显示角色: {role.IDCom.EntityName} ");
        }

        public void Hide(RoleEntity role) {
            role.LogicRoot.gameObject.SetActive(false);
            role.RendererRoot.gameObject.SetActive(false);
            TDLog.Log($"隐藏角色: {role.IDCom.EntityName} ");
        }

        public void ActivateCollider(RoleEntity role) {
            role.Coll_LogicRoot.enabled = true;
        }

        public void DeactivateCollider(RoleEntity role) {
            role.Coll_LogicRoot.enabled = false;
        }

        #endregion

        public void RecycleFieldRoles(int fieldTypeID) {
            var roleRepo = worldContext.RoleRepo;
            roleRepo.Foreach_ByFieldTypeID(fieldTypeID, (role) => {
                Hide(role);
            });
        }

        public void ResetAllAIs(int fieldTypeID, bool isShow) {
            var roleRepo = worldContext.RoleRepo;
            roleRepo.Foreach_ByFieldTypeID(fieldTypeID, (role) => {
                role.Reset();
                if (isShow) Show(role);
            });
        }
    }

}
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

        public RoleEntity SpawnRole(int fromFieldTypeID, EntityType entityType, int typeID, ControlType controlType, Vector3 pos, CampType campType, bool isBoss) {

            var factory = worldContext.Factory;
            bool has = factory.TryCreateRoleEntity(typeID, out var role);
            if (!has) {
                TDLog.Error($"创建角色失败! - {typeID}");
                return null;
            }

            BaseSetRole(role, typeID, pos, Quaternion.identity, campType, controlType);

            role.SetFromFieldTypeID(fromFieldTypeID);
            role.SetIsBoss(isBoss);

            var repo = worldContext.RoleRepo;
            var idCom = role.IDCom;
            if (idCom.ControlType == ControlType.Player) {
                repo.Set_Player(role);
            } else if (idCom.ControlType == ControlType.AI) {
                var ai = role.AIStrategy;
                ai.Activate();
                repo.Add_ToAI(role);
            }

            return role;
        }

        public bool TrySpawnRole(int fromFieldTypeID, in EntitySpawnModel entitySpawnModel, out RoleEntity role) {
            role = SpawnRole(fromFieldTypeID, entitySpawnModel.entityType, entitySpawnModel.typeID, entitySpawnModel.controlType, entitySpawnModel.spawnPos, entitySpawnModel.campType, entitySpawnModel.isBoss);
            return role != null;
        }

        public bool TrySummonRole(Vector3 summonPos, Quaternion summonRot, in EntityIDComponent summoner, in SkillSummonModel roleSummonModel, out RoleEntity role) {
            role = SpawnRole(summoner.FromFieldTypeID, EntityType.Role, roleSummonModel.typeID, roleSummonModel.controlType, summonPos, summoner.CampType, false);
            if (role != null) {
                var idCom = role.IDCom;
                idCom.SetFather(summoner);
                return true;
            } else {
                return false;
            }
        }

        public void ApplEffector(RoleEntity role, in EffectorEntity effectorModel) {
            var roleSelectorModel = effectorModel.roleEffectorModel.roleSelectorModel;
            var roleModifyModel = effectorModel.roleEffectorModel.roleModifyModel;
            var curFieldTypeID = worldContext.StateEntity.CurFieldTypeID;
            var roleRepo = worldContext.RoleRepo;

            roleRepo.Foreach_All((role) => {
                var attrCom = role.AttributeCom;
                if (!attrCom.IsMatch(roleSelectorModel)) return;
                ModifyRole(role.AttributeCom, roleModifyModel, 1);
            });
        }

        public void ModifyRole(RoleAttributeComponent attributeCom, RoleModifyModel attributeEffectModel, int stackCount) {
            // - HP
            var hpNCT = attributeEffectModel.hpNCT;
            var hpEV = attributeEffectModel.hpEV;
            if (hpNCT != NumCalculationType.None && hpEV != 0) {
                var curHPMax = attributeCom.HPMax;
                var curHP = attributeCom.HP;
                var offset = MathUtil.GetClampOffset(curHP, curHPMax, hpEV, 0, curHPMax, hpNCT);
                offset *= stackCount;
                curHP += offset;
                attributeEffectModel.hpOffset += offset;
                attributeCom.SetHP(curHP);
                TDLog.Log($"角色属性 HP 影响 ---> 值 {offset} => 当前 {attributeCom.HP}");
            }

            // - HPMax
            var hpMaxNCT = attributeEffectModel.hpMaxNCT;
            if (hpMaxNCT != NumCalculationType.None) {
                var hpMaxBase = attributeCom.HPMaxBase;
                var hpMaxEV = attributeEffectModel.hpMaxEV;
                var curHPMax = attributeCom.HPMax;
                var offset = MathUtil.GetClampOffset(curHPMax, hpMaxBase, hpMaxEV, 0, float.MaxValue, hpMaxNCT);
                offset *= stackCount;
                curHPMax += offset;
                attributeEffectModel.hpMaxOffset += offset;
                attributeCom.SetHPMax(curHPMax);
                TDLog.Log($"角色属性 HPMax 影响 --> 值 {offset} => 当前 {attributeCom.HPMax}");
            }

            // Move Speed
            var moveSpeedNCT = attributeEffectModel.moveSpeedNCT;
            if (moveSpeedNCT != NumCalculationType.None) {
                var moveSpeedBase = attributeCom.MoveSpeedBase;
                var curMoveSpeed = attributeCom.MoveSpeed;
                var finalMoveSpeed = curMoveSpeed;
                var moveSpeedEV = attributeEffectModel.moveSpeedEV;
                var offset = MathUtil.GetClampOffset(curMoveSpeed, moveSpeedBase, moveSpeedEV, 0, float.MaxValue, moveSpeedNCT);
                offset *= stackCount;
                finalMoveSpeed += offset;
                attributeEffectModel.moveSpeedOffset += offset;
                attributeCom.SetMoveSpeed(finalMoveSpeed);
                TDLog.Log($"角色属性 移动速度 影响 --> 值 {offset} => 当前 {attributeCom.MoveSpeed}");
            }

            // Normal Skill Speed
            var normalSkillSpeedBonusEV = attributeEffectModel.normalSkillSpeedBonusEV;
            if (normalSkillSpeedBonusEV != 0) {
                var offset = normalSkillSpeedBonusEV;
                offset *= stackCount;
                var curBonus = attributeCom.NormalSkillSpeedBonus + offset;
                attributeEffectModel.normalSkillSpeedBonusOffset += offset;
                attributeCom.SetNormalSkillSpeedBonus(curBonus);
                TDLog.Log($"角色属性 普技速度加成 影响 --> 值 {offset} => 当前 {attributeCom.NormalSkillSpeedBonus}");
            }


            // Damage Bonus
            var physicalDamageBonusEV = attributeEffectModel.physicalDamageBonusEV;
            if (normalSkillSpeedBonusEV != 0) {
                var offset = normalSkillSpeedBonusEV;
                offset *= stackCount;
                var curBonus = attributeCom.PhysicalDamageBonus + offset;
                attributeEffectModel.physicalDamageBonusOffset += offset;
                attributeCom.SetPhysicalDamageBonus(curBonus);
                TDLog.Log($"角色属性 物理伤害加成 影响 --> 值 {offset} => 当前 {attributeCom.PhysicalDamageBonus}");
            }

            var magicalDamageBonusEV = attributeEffectModel.magicalDamageBonusEV;
            if (magicalDamageBonusEV != 0) {
                var offset = magicalDamageBonusEV;
                offset *= stackCount;
                var curBonus = attributeCom.MagicalDamageBonus + offset;
                attributeEffectModel.magicalDamageBonusOffset += offset;
                attributeCom.SetmagicalDamageBonus(curBonus);
                TDLog.Log($"角色属性 魔法伤害加成 影响 --> 值 {offset} => 当前 {attributeCom.MagicalDamageBonus}");

            }

            //  Defence Bonus
            var physicalDefenseBonusEV = attributeEffectModel.physicalDefenseBonusEV;
            if (physicalDefenseBonusEV != 0) {
                var offset = physicalDefenseBonusEV;
                offset *= stackCount;
                var curBonus = attributeCom.PhysicalDefenseBonus + offset;
                attributeEffectModel.physicalDefenseBonusOffset += offset;
                attributeCom.SetPhysicalDefenseBonus(curBonus);
                TDLog.Log($"角色属性 物理减伤 影响 --> 值 {offset} => 当前 {attributeCom.PhysicalDefenseBonus}");

            }

            var magicalDefenseBonusEV = attributeEffectModel.magicalDefenseBonusEV;
            if (magicalDefenseBonusEV != 0) {
                var offset = magicalDefenseBonusEV;
                offset *= stackCount;
                var curBonus = attributeCom.MagicalDefenseBonus + offset;
                attributeEffectModel.magicalDefenseBonusOffset += offset;
                attributeCom.SetMagicalDefenseBonus(curBonus);
                TDLog.Log($"角色属性 魔法减伤 影响 --> 值 {offset} => 当前 {attributeCom.MagicalDefenseBonus}");
            }

        }

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

            // AI
            if (controlType == ControlType.AI) {
                var factory = worldContext.Factory;
                var ai = factory.CreateAIStrategy(role, typeID);
                role.SetAIStrategy(ai);
            }

        }

        /// <summary>
        /// 玩家角色输入
        /// </summary>
        public void BakePlayerInput() {
            RoleEntity playerRole = worldContext.RoleRepo.PlayerRole;
            if (playerRole == null) {
                return;
            }

            var inputCom = playerRole.InputCom;
            var inputGetter = infraContext.InputCore.Getter;

            // - Move
            Vector2 moveAxis;
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
            inputCom.SetMoveAxis(moveAxis);

            // - Jump
            if (inputGetter.GetDown(InputKeyCollection.JUMP)) {
                inputCom.SetInputJump(true);
            }

            // - Normal SKill
            if (inputGetter.GetDown(InputKeyCollection.MELEE)) {
                inputCom.SetInputSkillMelee(true);
            }

            // - Special Skill
            if (inputGetter.GetDown(InputKeyCollection.SKILL1)) {
                inputCom.SetInputSpecialSkill(true);
            }

            // - Ultimate Skill
            if (inputGetter.GetDown(InputKeyCollection.SKILL2)) {
                inputCom.SetInputUltimateSkill(true);
            }

            // - Dash Skill
            if (inputGetter.GetDown(InputKeyCollection.DASH)) {
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

        public void Dash(RoleEntity role, Vector2 dir, Vector2 force) {
            var moveCom = role.MoveCom;
            moveCom.Dash(dir, force);
        }

        /// <summary>
        /// 角色释放技能
        /// </summary>
        public bool TryCastSkillByInput(RoleEntity role) {
            var inputCom = role.InputCom;
            SkillCastKey inputCastKey = inputCom.GetSkillCastKey();
            return TryCastSkill(role, inputCastKey);
        }

        public bool TryCastSkill(RoleEntity role, SkillCastKey inputCastKey) {
            if (inputCastKey == SkillCastKey.None) {
                return false;
            }

            var skillSlotCom = role.SkillSlotCom;
            if (!skillSlotCom.TrgGetByCastKey(inputCastKey, out var originalSkill)) {
                TDLog.Error($"施放技能失败 - 不存在原始技能类型 {inputCastKey}");
                return false;
            }

            int originSkillTypeID = originalSkill.IDCom.TypeID;
            var roleFSMDomain = this.worldContext.RootDomain.RoleFSMDomain;

            // 正常释放
            var fsmCom = role.FSMCom;
            if (fsmCom.FSMState == RoleFSMState.Idle) {
                roleFSMDomain.Enter_Casting(role, originalSkill);
                return true;
            }

            // 连招
            if (fsmCom.FSMState == RoleFSMState.Casting) {
                var stateModel = fsmCom.CastingStateModel;
                var castingSkill = stateModel.CastingSkill;

                if (CanCancelSkill(skillSlotCom, castingSkill, originSkillTypeID, out var realSkillTypeID)) {
                    castingSkill.Reset();
                    roleFSMDomain.Enter_Casting(role, realSkillTypeID);
                    return true;
                }
            }

            return false;
        }

        bool CanCancelSkill(SkillSlotComponent skillSlotCom, SkillEntity castingSkill, int inputSkillTypeID, out SkillEntity realSkill) {
            realSkill = null;

            if (castingSkill.OriginalSkillTypeID == inputSkillTypeID) {
                // 组合技
                bool isComboSkill = false;
                SkillEntity comboSkill = null;
                castingSkill.Foreach_CancelModel_Linked((System.Action<SkillCancelModel>)((cancelModel) => {
                    if (cancelModel.cancelType != SkillCancelType.Combo) {
                        return;
                    }
                    int skillTypeID = cancelModel.skillTypeID;
                    if (!skillSlotCom.TryGet(skillTypeID, out comboSkill)) return;
                    isComboSkill = true;
                }));
                if (isComboSkill) {
                    realSkill = comboSkill;
                    return true;
                }
            } else {
                // 连招技
                bool isLinkedSkill = false;
                SkillEntity linkedSkill = null;
                castingSkill.Foreach_CancelModel_Linked((System.Action<SkillCancelModel>)((cancelModel) => {
                    int skillTypeID = cancelModel.skillTypeID;
                    if (!skillSlotCom.TryGet(skillTypeID, out linkedSkill)) return;
                    isLinkedSkill = true;
                }));
                if (isLinkedSkill) {
                    realSkill = linkedSkill;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 角色受击的统一处理方式
        /// </summary>
        public void HandleBeHit(int hitFrame, Vector2 beHitDir, RoleEntity role, in EntityIDComponent hitter, in ColliderToggleModel collisionTriggerModel) {
            var rootDomain = worldContext.RootDomain;
            var roleFSMDomain = rootDomain.RoleFSMDomain;
            var roleDomain = rootDomain.RoleDomain;
            var effectorDomain = rootDomain.EffectorDomain;

            // 受击
            var beHitModel = collisionTriggerModel.beHitModel;
            role.FSMCom.Enter_BeHit(beHitDir, beHitModel);

            // 伤害 仲裁
            var damageArbitService = worldContext.DamageArbitService;
            var damageModel = collisionTriggerModel.damageModel;
            var baseDamage = damageModel.GetDamage(hitFrame);
            var damageType = damageModel.damageType;
            float realDamage = baseDamage;

            // Hitter 可能是: SkillEntity or BulletEntity
            RoleEntity hitterRole = hitter.Father.HolderPtr as RoleEntity;
            if (hitterRole != null) {
                realDamage = damageArbitService.ArbitrateDamage(damageType, baseDamage, hitterRole.AttributeCom, role.AttributeCom);
            }

            // 伤害结算
            roleDomain.ReduceHP(role, realDamage);
            damageArbitService.Add(damageModel.damageType, realDamage, role.IDCom, hitter);

        }

        public void TearDownRole(RoleEntity role) {
            TDLog.Log($"角色 TearDown - {role.IDCom.TypeID}");
            role.Reset();
            role.AttributeCom.ClearHP();
            role.SetColliderActive(false);
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
                if (isShow) {
                    role.Show();
                }
            });
        }

        #region [Attribute]

        public float ReduceHP(RoleEntity role, float damage) {
            var attributeCom = role.AttributeCom;
            var decrease = attributeCom.ReduceHP(damage);

            TDLog.Log($"{role.IDCom.EntityName} 受到伤害 {damage} HP减少: {decrease}");
            return decrease;
        }

        public float ReduceHP_Percentage(RoleEntity role, float percentage) {
            var attributeCom = role.AttributeCom;

            var curHP = attributeCom.HP;
            var decrease = curHP * percentage;
            decrease = attributeCom.ReduceHP(decrease);

            TDLog.Log($"{role.IDCom.EntityName} 受到百分比伤害 百分比: {percentage} HP减少: {decrease}");
            return decrease;
        }

        #endregion

    }

}
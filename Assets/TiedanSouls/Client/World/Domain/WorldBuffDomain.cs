using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;
using System;

namespace TiedanSouls.Client.Domain {

    public class WorldBuffDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldBuffDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public void TickAllBuff(int curFieldTypeID, float dt) {
            var buffDomain = worldContext.RootDomain.BuffDomain;
            worldContext.RoleRepo.Foreach_AI(curFieldTypeID, (role) => {
                buffDomain.TickRoleBuff(role, dt);
            });
            var playerRole = worldContext.RoleRepo.PlayerRole;
            if (playerRole != null) {
                buffDomain.TickRoleBuff(playerRole, dt);
            }
        }

        void TickRoleBuff(RoleEntity role, float dt) {
            var rootDomain = worldContext.RootDomain;
            var buffDomain = rootDomain.BuffDomain;
            var buffSlotCom = role.BuffSlotCom;
            var attributeCom = role.AttributeCom;

            var removeList = buffSlotCom.ForeachAndGetRemoveList((buff) => {
                buff.AddCurFrame();
                if (!buff.IsTriggerFrame()) {
                    return;
                }

                var stackCount = buff.ExtraStackCount + 1;
                ModifyRole(attributeCom, buff.RoleAttrModifyModel, stackCount);
            });

            var buffRepo = worldContext.BuffRepo;
            removeList.ForEach((buff) => {
                buffDomain.TryRevokeBuff(buff, role.AttributeCom);
                buffSlotCom.TryRemove(buff);
                buffRepo.TryRemoveToPool(buff);
                buff.ResetAll();
            });
        }

        public bool TryAttachBuff(in EntityIDArgs father, in EntityIDArgs target, in BuffAttachModel buffAttachModel, out BuffEntity buff) {
            buff = null;

            var targetEntityType = target.entityType;
            if (targetEntityType == EntityType.Role) {
                var roleRepo = worldContext.RoleRepo;
                if (!roleRepo.TryGet_FromAll(target.entityID, out var targetRole)) {
                    TDLog.Error($"附加Buff失败, 目标角色不存在:{target.entityID}");
                    return false;
                }

                // Buff叠加 & 替换
                var buffTypeID = buffAttachModel.buffID;
                var buffSlotCom = targetRole.BuffSlotCom;
                if (buffSlotCom.TryGet(buffTypeID, out buff)) {
                    if (buff.ExtraStackCount < buff.MaxExtraStackCount) {
                        buff.AddExtraStackCount();
                        TDLog.Log($"Buff[{buff.IDCom.TypeID}]叠加 当前层数:{buff.ExtraStackCount + 1}");
                    }

                    TryRevokeBuff(buff, targetRole.AttributeCom);
                    buff.ResetTriggerTimes();
                    buff.ResetCurFrame();
                    buff.RoleAttrModifyModel.ResetOffset();
                    return true;
                }

                if (!TrySpawn(buffTypeID, father, out buff)) {
                    TDLog.Error($"附加Buff失败, 生成Buff失败:{buffTypeID}");
                    return false;
                }

                buff.SetFather(father);
                buffSlotCom.TryAdd(buff);
                TDLog.Log($"Buff[{buff.IDCom.TypeID}]附加成功");

                return true;
            }

            TDLog.Error($"召唤Buff失败, 目标类型不支持:{targetEntityType}");
            return false;
        }

        public bool TrySpawn(int typeID, in EntityIDArgs father, out BuffEntity buff) {
            buff = null;

            var buffRepo = worldContext.BuffRepo;
            bool isFromPool = buffRepo.TryGetFromPool(typeID, out buff);
            if (!isFromPool) {
                var factory = worldContext.Factory;
                if (!factory.TryCreateBuff(typeID, out buff)) {
                    return false;
                }
            }

            // ID
            var idCom = buff.IDCom;
            if (isFromPool) {
                buff.ResetAll();
            } else {
                var entityID = worldContext.IDService.PickBuffID();
                idCom.SetEntityID(entityID);
            }
            idCom.SetFather(father);

            var repo = worldContext.BuffRepo;
            repo.Add(buff);

            return true;
        }

        public bool TryRevokeBuff(BuffEntity buff, RoleAttributeComponent attributeCom) {
            var attributeEffectModel = buff.RoleAttrModifyModel;
            var needRevoke = buff.NeedRevoke;
            if (!needRevoke) {
                return false;
            }

            var hpOffset = attributeEffectModel.hpOffset;
            if (hpOffset != 0) {
                var hp = attributeCom.HP;
                hp = Math.Min(1, hp - hpOffset);
                attributeCom.SetHP(hp);
                TDLog.Log($"Buff HP 撤销 --> 值 {hpOffset} => 当前 {attributeCom.HP}");
            }

            var hpMax = attributeCom.HPMax;
            var hpMaxOffset = attributeEffectModel.hpMaxOffset;
            if (hpMaxOffset != 0) {
                attributeCom.SetHPMax(hpMax - hpMaxOffset);
                TDLog.Log($"Buff HPMax 撤销 --> 值 {hpMaxOffset} => 当前 {attributeCom.HPMax}");
            }

            var moveSpeed = attributeCom.MoveSpeed;
            var moveSpeedOffset = attributeEffectModel.moveSpeedOffset;
            if (moveSpeedOffset != 0) {
                attributeCom.SetMoveSpeed(moveSpeed - moveSpeedOffset);
                TDLog.Log($"Buff 移动速度 撤销 --> 值 {moveSpeedOffset} => 当前 {attributeCom.MoveSpeed}");
            }

            var normalSkillSpeedBonus = attributeCom.NormalSkillSpeedBonus;
            var normalSkillSpeedBonusOffset = attributeEffectModel.normalSkillSpeedBonusOffset;
            if (normalSkillSpeedBonusOffset != 0) {
                attributeCom.SetNormalSkillSpeedBonus(normalSkillSpeedBonus - normalSkillSpeedBonusOffset);
                TDLog.Log($"Buff 普技速度加成 撤销 --> 值 {normalSkillSpeedBonusOffset}=> 当前 {attributeCom.NormalSkillSpeedBonus}");
            }

            var physicalDamageBonus = attributeCom.PhysicalDamageBonus;
            var physicalDamageBonusOffset = attributeEffectModel.physicalDamageBonusOffset;
            if (physicalDamageBonusOffset != 0) {
                attributeCom.SetPhysicalDamageBonus(physicalDamageBonus - physicalDamageBonusOffset);
                TDLog.Log($"Buff 物理加伤加成 撤销 --> 值 {physicalDamageBonusOffset}=> 当前 {attributeCom.PhysicalDamageBonus}");
            }

            var magicDamageBonus = attributeCom.MagicalDamageBonus;
            var magicalDamageBonusOffset = attributeEffectModel.magicalDamageBonusOffset;
            if (magicalDamageBonusOffset != 0) {
                attributeCom.SetmagicalDamageBonus(magicDamageBonus - magicalDamageBonusOffset);
                TDLog.Log($"Buff 魔法加伤加成 撤销 --> 值 {magicalDamageBonusOffset}=> 当前 {attributeCom.MagicalDamageBonus}");
            }

            var physicalDefenseBonus = attributeCom.PhysicalDefenseBonus;
            var physicalDefenseBonusOffset = attributeEffectModel.physicalDefenseBonusOffset;
            if (physicalDefenseBonusOffset != 0) {
                attributeCom.SetPhysicalDefenseBonus(physicalDefenseBonus - physicalDefenseBonusOffset);
                TDLog.Log($"Buff 物理防御加成 撤销 --> 值 {physicalDefenseBonusOffset}=> 当前 {attributeCom.PhysicalDefenseBonus}");
            }

            var magicDefenseBonus = attributeCom.MagicalDefenseBonus;
            var magicalDefenseBonusOffset = attributeEffectModel.magicalDefenseBonusOffset;
            if (magicalDefenseBonusOffset != 0) {
                attributeCom.SetMagicalDefenseBonus(magicDefenseBonus - magicalDefenseBonusOffset);
                TDLog.Log($"Buff 魔法防御加成 撤销 --> 值 {magicalDefenseBonusOffset} => 当前 {attributeCom.MagicalDefenseBonus}");
            }

            return true;
        }

        public void ModifyRole(RoleAttributeComponent attributeCom, RoleAttributeModifyModel attributeEffectModel, int stackCount) {
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

    }

}
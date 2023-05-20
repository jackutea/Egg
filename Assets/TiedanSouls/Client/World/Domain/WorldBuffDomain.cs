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
            var removeList = buffSlotCom.ForeachAndGetRemoveList((buff) => {
                buff.AddCurFrame();
                buffDomain.TryEffectRoleAttribute(role.AttributeCom, buff);
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
                    TDLog.Error($"召唤Buff失败, 目标角色不存在:{target.entityID}");
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
                    buff.RoleAttributeEffectModel.ResetOffset();
                    return true;
                }

                if (!TrySpawn(buffTypeID, father, out buff)) {
                    TDLog.Error($"召唤Buff失败, 生成Buff失败:{buffTypeID}");
                    return false;
                }

                buff.SetFather(father);
                buffSlotCom.TryAdd(buff);

                return true;
            }

            TDLog.Warning($"召唤Buff失败, 目标类型不支持:{targetEntityType}");
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
            var attributeEffectModel = buff.RoleAttributeEffectModel;
            var needRevoke = buff.NeedRevoke;
            if (!needRevoke) {
                return false;
            }

            var hp = attributeCom.HP;
            var offset = attributeEffectModel.hpOffset;
            hp = Math.Min(1, hp - offset);
            attributeCom.SetHP(hp);
            TDLog.Log($"Buff HP 撤销 --> 值 {offset} => 当前 {attributeCom.HP}");

            var hpMax = attributeCom.HPMax;
            offset = attributeEffectModel.hpMaxOffset;
            attributeCom.SetHPMax(hpMax - offset);
            TDLog.Log($"Buff HPMax 撤销 --> 值 {offset} => 当前 {attributeCom.HPMax}");

            var moveSpeed = attributeCom.MoveSpeed;
            offset = attributeEffectModel.moveSpeedOffset;
            attributeCom.SetMoveSpeed(moveSpeed - offset);
            TDLog.Log($"Buff 移动速度 撤销 --> 值 {offset} => 当前 {attributeCom.MoveSpeed}");

            var normalSkillSpeedBonus = attributeCom.NormalSkillSpeedBonus;
            offset = attributeEffectModel.normalSkillSpeedBonusOffset;
            attributeCom.SetNormalSkillSpeedBonus(normalSkillSpeedBonus - offset);
            TDLog.Log($"Buff 普技速度加成 撤销 --> 值 {offset}=> 当前 {attributeCom.NormalSkillSpeedBonus}");

            var physicalDamageBonus = attributeCom.PhysicalDamageBonus;
            offset = attributeEffectModel.physicalDamageBonusOffset;
            attributeCom.SetPhysicalDamageBonus(physicalDamageBonus - offset);
            TDLog.Log($"Buff 物理加伤加成 撤销 --> 值 {offset}=> 当前 {attributeCom.PhysicalDamageBonus}");

            var magicDamageBonus = attributeCom.MagicalDamageBonus;
            offset = attributeEffectModel.magicalDamageBonusOffset;
            attributeCom.SetmagicalDamageBonus(magicDamageBonus - offset);
            TDLog.Log($"Buff 魔法加伤加成 撤销 --> 值 {offset}=> 当前 {attributeCom.MagicalDamageBonus}");

            var physicalDefenseBonus = attributeCom.PhysicalDefenseBonus;
            offset = attributeEffectModel.physicalDefenseBonusOffset;
            attributeCom.SetPhysicalDefenseBonus(physicalDefenseBonus - offset);
            TDLog.Log($"Buff 物理防御加成 撤销 --> 值 {offset}=> 当前 {attributeCom.PhysicalDefenseBonus}");

            var magicDefenseBonus = attributeCom.MagicalDefenseBonus;
            offset = attributeEffectModel.magicalDefenseBonusOffset;
            attributeCom.SetMagicalDefenseBonus(magicDefenseBonus - offset);
            TDLog.Log($"Buff 魔法防御加成 撤销 --> 值 {offset} => 当前 {attributeCom.MagicalDefenseBonus}");

            return true;
        }

        public bool TryEffectRoleAttribute(RoleAttributeComponent attributeCom, BuffEntity buff) {
            if (!buff.IsTriggerFrame()) {
                return false;
            }

            var stackCount = buff.ExtraStackCount + 1;
            ModifyRole(attributeCom, buff.RoleAttributeEffectModel, stackCount);

            return true;
        }

        public void ModifyRole(RoleAttributeComponent attributeCom, in RoleAttributeModifyModel attributeEffectModel, int stackCount) {
            var offset = 0f;
            var ev = 0f;
            var curBonus = 0f;

            // - HP
            var curHPMax = attributeCom.HPMax;
            var hpEV = attributeEffectModel.hpEV;
            var hpNCT = attributeEffectModel.hpNCT;
            var curHP = attributeCom.HP;
            offset = MathUtil.GetClampOffset(curHP, curHPMax, hpEV, 0, curHPMax, hpNCT);
            offset *= stackCount;
            curHP += offset;
            attributeEffectModel.hpOffset += offset;
            attributeCom.SetHP(curHP);
            TDLog.Log($"角色属性 HP 影响 ---> 值 {offset} => 当前 {attributeCom.HP}");

            // - HPMax
            var hpMaxBase = attributeCom.HPMaxBase;
            var hpMaxEV = attributeEffectModel.hpMaxEV;
            var hpMaxNCT = attributeEffectModel.hpMaxNCT;
            offset = MathUtil.GetClampOffset(curHPMax, hpMaxBase, hpMaxEV, 0, float.MaxValue, hpMaxNCT);
            offset *= stackCount;
            curHPMax += offset;
            attributeEffectModel.hpMaxOffset += offset;
            attributeCom.SetHPMax(curHPMax);
            TDLog.Log($"角色属性 HPMax 影响 --> 值 {offset} => 当前 {attributeCom.HPMax}");

            // Move Speed
            var moveSpeedBase = attributeCom.MoveSpeedBase;
            var curMoveSpeed = attributeCom.MoveSpeed;
            var finalMoveSpeed = curMoveSpeed;
            var moveSpeedEV = attributeEffectModel.moveSpeedEV;
            var moveSpeedNCT = attributeEffectModel.moveSpeedNCT;
            offset = MathUtil.GetClampOffset(curMoveSpeed, moveSpeedBase, moveSpeedEV, 0, float.MaxValue, moveSpeedNCT);
            offset *= stackCount;
            finalMoveSpeed += offset;
            attributeEffectModel.moveSpeedOffset += offset;
            attributeCom.SetMoveSpeed(finalMoveSpeed);
            TDLog.Log($"角色属性 移动速度 影响 --> 值 {offset} => 当前 {attributeCom.MoveSpeed}");

            // Normal Skill Speed
            ev = attributeEffectModel.normalSkillSpeedBonusEV;
            offset = ev;
            offset *= stackCount;
            curBonus = attributeCom.NormalSkillSpeedBonus + offset;
            attributeEffectModel.normalSkillSpeedBonusOffset += offset;
            attributeCom.SetNormalSkillSpeedBonus(curBonus);
            TDLog.Log($"角色属性 普技速度加成 影响 --> 值 {offset} => 当前 {attributeCom.NormalSkillSpeedBonus}");

            // Damage Bonus
            ev = attributeEffectModel.physicalDamageBonusEV;
            offset = ev;
            offset *= stackCount;
            curBonus = attributeCom.PhysicalDamageBonus + offset;
            attributeEffectModel.physicalDamageBonusOffset += offset;
            attributeCom.SetPhysicalDamageBonus(curBonus);
            TDLog.Log($"角色属性 物理伤害加成 影响 --> 值 {offset} => 当前 {attributeCom.PhysicalDamageBonus}");

            ev = attributeEffectModel.magicalDamageBonusEV;
            offset = ev;
            offset *= stackCount;
            curBonus = attributeCom.MagicalDamageBonus + offset;
            attributeEffectModel.magicalDamageBonusOffset += offset;
            attributeCom.SetmagicalDamageBonus(curBonus);
            TDLog.Log($"角色属性 魔法伤害加成 影响 --> 值 {offset} => 当前 {attributeCom.MagicalDamageBonus}");

            //  Defence Bonus
            ev = attributeEffectModel.physicalDefenseBonusEV;
            offset = ev;
            offset *= stackCount;
            curBonus = attributeCom.PhysicalDefenseBonus + offset;
            attributeEffectModel.physicalDefenseBonusOffset += offset;
            attributeCom.SetPhysicalDefenseBonus(curBonus);
            TDLog.Log($"角色属性 物理减伤 影响 --> 值 {offset} => 当前 {attributeCom.PhysicalDefenseBonus}");

            ev = attributeEffectModel.magicalDefenseBonusEV;
            offset = ev;
            offset *= stackCount;
            curBonus = attributeCom.MagicalDefenseBonus + offset;
            attributeEffectModel.magicalDefenseBonusOffset += offset;
            attributeCom.SetMagicalDefenseBonus(curBonus);
            TDLog.Log($"角色属性 魔法减伤 影响 --> 值 {offset} => 当前 {attributeCom.MagicalDefenseBonus}");
        }

    }

}
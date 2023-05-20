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
            var roleDomain = rootDomain.RoleDomain;
            var buffDomain = rootDomain.BuffDomain;
            var buffSlotCom = role.BuffSlotCom;
            var attributeCom = role.AttributeCom;

            var removeList = buffSlotCom.ForeachAndGetRemoveList((buff) => {
                buff.AddCurFrame();
                if (!buff.CanTrigger()) {
                    return;
                }

                var stackCount = buff.ExtraStackCount + 1;
                roleDomain.ModifyRole(attributeCom, buff.RoleAttrModifyModel, stackCount);
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

            var hpMax = attributeCom.HPMax;
            var hpMaxOffset = attributeEffectModel.hpMaxOffset;
            if (hpMaxOffset != 0) {
                attributeCom.SetHPMax(hpMax - hpMaxOffset);
                TDLog.Log($"角色属性 撤销 HPMax --> 值 {hpMaxOffset} => 当前 {attributeCom.HPMax}");
            }

            var moveSpeed = attributeCom.MoveSpeed;
            var moveSpeedOffset = attributeEffectModel.moveSpeedOffset;
            if (moveSpeedOffset != 0) {
                attributeCom.SetMoveSpeed(moveSpeed - moveSpeedOffset);
                TDLog.Log($"角色属性 撤销 移动速度 --> 值 {moveSpeedOffset} => 当前 {attributeCom.MoveSpeed}");
            }

            var normalSkillSpeedBonus = attributeCom.NormalSkillSpeedBonus;
            var normalSkillSpeedBonusOffset = attributeEffectModel.normalSkillSpeedBonusOffset;
            if (normalSkillSpeedBonusOffset != 0) {
                attributeCom.SetNormalSkillSpeedBonus(normalSkillSpeedBonus - normalSkillSpeedBonusOffset);
                TDLog.Log($"角色属性 撤销 普技速度加成 --> 值 {normalSkillSpeedBonusOffset}=> 当前 {attributeCom.NormalSkillSpeedBonus}");
            }

            var physicalDamageBonus = attributeCom.PhysicalDamageBonus;
            var physicalDamageBonusOffset = attributeEffectModel.physicalDamageBonusOffset;
            if (physicalDamageBonusOffset != 0) {
                attributeCom.SetPhysicalDamageBonus(physicalDamageBonus - physicalDamageBonusOffset);
                TDLog.Log($"角色属性 撤销 物理加伤加成 --> 值 {physicalDamageBonusOffset}=> 当前 {attributeCom.PhysicalDamageBonus}");
            }

            var magicDamageBonus = attributeCom.MagicalDamageBonus;
            var magicalDamageBonusOffset = attributeEffectModel.magicalDamageBonusOffset;
            if (magicalDamageBonusOffset != 0) {
                attributeCom.SetmagicalDamageBonus(magicDamageBonus - magicalDamageBonusOffset);
                TDLog.Log($"角色属性 撤销 魔法加伤加成 --> 值 {magicalDamageBonusOffset}=> 当前 {attributeCom.MagicalDamageBonus}");
            }

            var physicalDefenseBonus = attributeCom.PhysicalDefenseBonus;
            var physicalDefenseBonusOffset = attributeEffectModel.physicalDefenseBonusOffset;
            if (physicalDefenseBonusOffset != 0) {
                attributeCom.SetPhysicalDefenseBonus(physicalDefenseBonus - physicalDefenseBonusOffset);
                TDLog.Log($"角色属性 撤销 物理防御加成 --> 值 {physicalDefenseBonusOffset}=> 当前 {attributeCom.PhysicalDefenseBonus}");
            }

            var magicDefenseBonus = attributeCom.MagicalDefenseBonus;
            var magicalDefenseBonusOffset = attributeEffectModel.magicalDefenseBonusOffset;
            if (magicalDefenseBonusOffset != 0) {
                attributeCom.SetMagicalDefenseBonus(magicDefenseBonus - magicalDefenseBonusOffset);
                TDLog.Log($"角色属性 撤销 魔法防御加成 --> 值 {magicalDefenseBonusOffset} => 当前 {attributeCom.MagicalDefenseBonus}");
            }

            return true;
        }

    }

}
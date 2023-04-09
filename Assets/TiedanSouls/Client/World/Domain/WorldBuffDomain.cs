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
        WorldRootDomain rootDomain;

        public WorldBuffDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = worldDomain;
        }

        /// <summary>
        /// 根据实体召唤模型
        /// </summary>
        public bool TrySummon(in EntityIDArgs summoner, in EntitySummonModel entitySummonModel, out BuffEntity buff) {
            buff = null;
            RoleEntity roleFather = null;
            if (!rootDomain.TryFindRoleFather(summoner, ref roleFather)) {
                TDLog.Error($"召唤Buff失败, 召唤者角色找不到:{summoner}");
                return false;
            }

            var buffTypeID = entitySummonModel.typeID;
            if (!TrySpawn(buffTypeID, summoner, out buff)) {
                TDLog.Error($"召唤Buff失败, 生成Buff失败:{buffTypeID}");
                return false;
            }

            // TODO: Buff叠加和替换的业务逻辑
            buff.SetFather(roleFather.IDCom.ToArgs());
            roleFather.BuffSlotCom.Add(buff);
            return true;
        }

        /// <summary>
        /// 根据实体生成模型 生成Buff
        /// </summary>
        public bool TrySpawnBySpawnModel(int fromFieldTypeID, in EntitySpawnModel entitySpawnModel, out BuffEntity buff) {
            buff = null;

            var typeID = entitySpawnModel.typeID;
            var factory = worldContext.Factory;
            if (!factory.TryCreateBuff(typeID, out buff)) {
                return false;
            }

            var spawnPos = entitySpawnModel.spawnPos;
            var spawnControlType = entitySpawnModel.controlType;
            var spawnAllyType = entitySpawnModel.allyType;

            // Buff ID
            var idCom = buff.IDCom;
            idCom.SetEntityID(worldContext.IDService.PickBuffID());
            idCom.SetAllyType(spawnAllyType);
            idCom.SetControlType(spawnControlType);

            // 添加至仓库
            var repo = worldContext.BuffRepo;
            repo.Add(buff);
            return true;
        }

        /// <summary>
        /// 根据类型ID生成Buff
        /// </summary>
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
                buff.Reset();
            } else {
                var entityID = worldContext.IDService.PickBuffID();
                idCom.SetEntityID(entityID);
            }
            idCom.SetFather(father);

            var repo = worldContext.BuffRepo;
            repo.Add(buff);

            return true;
        }

        /// <summary>
        /// 撤销对属性值的影响,并回收Buff
        /// </summary>
        public void RevokeBuff(BuffEntity buff, AttributeComponent attributeCom) {
            var attributeEffectModel = buff.AttributeEffectModel;
            if (attributeEffectModel.hpNeedRevoke) {
                var hp = attributeCom.HP;
                var offset = attributeEffectModel.hpOffset;
                hp = Math.Min(1, hp - offset);
                attributeCom.SetHP(hp);
                TDLog.Log($"Buff 撤销 HP --> 变化 {offset} => 当前 {attributeCom.HP}");
            }
            if (attributeEffectModel.hpMaxNeedRevoke) {
                var hpMax = attributeCom.HPMax;
                var offset = attributeEffectModel.hpMaxOffset;
                attributeCom.SetHPMax(hpMax - offset);
                TDLog.Log($"Buff 撤销 HPMax --> 变化 {offset} => 当前 {attributeCom.HPMax}");
            }
            if (attributeEffectModel.moveSpeedRevoke) {
                var moveSpeed = attributeCom.MoveSpeed;
                var offset = attributeEffectModel.moveSpeedOffset;
                attributeCom.SetMoveSpeed(moveSpeed - offset);
                TDLog.Log($"Buff 撤销, 移动速度 => -{offset} => 当前 {attributeCom.MoveSpeed}");
            }
            if (attributeEffectModel.normalSkillSpeedBonusNeedRevoke) {
                var normalSkillSpeedBonus = attributeCom.NormalSkillSpeedBonus;
                var offset = attributeEffectModel.normalSkillSpeedBonusOffset;
                attributeCom.SetNormalSkillSpeedBonus(normalSkillSpeedBonus - offset);
                TDLog.Log($"Buff 撤销, 普攻速度加成 => -{offset}=> 当前 {attributeCom.NormalSkillSpeedBonus}");
            }
            if (attributeEffectModel.physicalDamageBonusNeedRevoke) {
                var physicalDamageBonus = attributeCom.PhysicalDamageBonus;
                var offset = attributeEffectModel.physicalDamageBonusOffset;
                attributeCom.SetPhysicalDamageBonus(physicalDamageBonus - offset);
                TDLog.Log($"Buff 撤销, 物理加伤加成 => -{offset}=> 当前 {attributeCom.PhysicalDamageBonus}");
            }
            if (attributeEffectModel.magicalDamageBonusNeedRevoke) {
                var magicDamageBonus = attributeCom.MagicalDamageBonus;
                var offset = attributeEffectModel.magicalDamageBonusOffset;
                attributeCom.SetmagicalDamageBonus(magicDamageBonus - offset);
                TDLog.Log($"Buff 撤销, 魔法加伤加成 => -{offset}=> 当前 {attributeCom.MagicalDamageBonus}");
            }
            if (attributeEffectModel.needRevokePhysicalDefenseBonus) {
                var physicalDefenseBonus = attributeCom.PhysicalDefenseBonus;
                var offset = attributeEffectModel.physicalDefenseBonusOffset;
                attributeCom.SetPhysicalDefenseBonus(physicalDefenseBonus - offset);
                TDLog.Log($"Buff 撤销, 物理防御加成 => -{offset}=> 当前 {attributeCom.PhysicalDefenseBonus}");
            }
            if (attributeEffectModel.magicalDefenseBonusNeedRevoke) {
                var magicDefenseBonus = attributeCom.MagicalDefenseBonus;
                var offset = attributeEffectModel.magicalDefenseBonusOffset;
                attributeCom.SetMagicalDefenseBonus(magicDefenseBonus - offset);
                TDLog.Log($"Buff 撤销, 魔法防御加成 => -{offset} => 当前 {attributeCom.MagicalDefenseBonus}");
            }
        }

        public bool TryTriggerEffector(BuffEntity buff) {
            if (!buff.IsTriggerFrame()) {
                return false;
            }

            var effectorTypeID = buff.EffectorTypeID;
            var effectorDomain = rootDomain.EffectorDomain;
            if (!effectorDomain.TrySpawnEffectorModel(effectorTypeID, out var effectorModel)) {
                return false;
            }

            var idCom = buff.IDCom;
            var father = idCom.Father;
            Vector3 summonPos = Vector3.zero;
            Quaternion baseRot = Quaternion.identity;
            EntityIDArgs summoner = father;

            if (this.rootDomain.TryGetEntityObj(idCom.Father, out var entity)) {
                if (entity is RoleEntity role) {
                    summonPos = role.LogicRootPos;
                    baseRot = role.LogicRotation;
                } else {
                    TDLog.Error($"Buff 效果器触发失败, 父实体不是角色实体");
                    return false;
                }
            }

            var entitySummonModelArray = effectorModel.entitySummonModelArray;
            var entityDestroyModelArray = effectorModel.entityDestroyModelArray;
            this.rootDomain.SpawnBy_EntitySummonModelArray(summonPos, baseRot, summoner, entitySummonModelArray);
            this.rootDomain.DestroyBy_EntityDestroyModelArray(summoner, entityDestroyModelArray);
            TDLog.Log($"Buff[{idCom.EntityName}] ======> 效果器触发效果器:{effectorModel.effectorName}");
            return true;
        }

        public bool TryTriggerAttributeEffect(AttributeComponent attributeCom, BuffEntity buff) {
            if (!buff.IsTriggerFrame()) {
                return false;
            }

            buff.triggerTimes++;
            var attributeEffectModel = buff.AttributeEffectModel;

            // - HP
            var curHPMax = attributeCom.HPMax;
            var hpMaxBase = attributeCom.HPMaxBase;
            var hpEV = attributeEffectModel.hpEV;
            var hpNCT = attributeEffectModel.hpNCT;
            var hpEffectTimes = attributeEffectModel.hpEffectTimes;
            if (hpNCT != NumCalculationType.None && buff.triggerTimes <= hpEffectTimes) {
                var offset = 0f;
                var curHP = attributeCom.HP;
                if (hpNCT == NumCalculationType.PercentageAdd) {
                    offset = Mathf.RoundToInt(hpMaxBase * hpEV);
                    curHP += offset;
                } else if (hpNCT == NumCalculationType.PercentageMul) {
                    offset = Mathf.RoundToInt(curHPMax * hpEV);
                    curHP += offset;
                } else if (hpNCT == NumCalculationType.AbsoluteAdd) {
                    offset = hpEV;
                    curHP += offset;
                }
                attributeEffectModel.hpOffset += offset;
                attributeCom.SetHP(curHP);
                TDLog.Log($"Buff 影响 HP ---> 变化 {offset} => 当前 {attributeCom.HP}");
            }

            // - HPMax
            var hpMaxEV = attributeEffectModel.hpMaxEV;
            var hpMaxNCT = attributeEffectModel.hpMaxNCT;
            var hpMaxEffectTimes = attributeEffectModel.hpMaxEffectTimes;
            if (hpMaxNCT != NumCalculationType.None && buff.triggerTimes <= hpMaxEffectTimes) {
                var offset = 0f;
                if (hpMaxNCT == NumCalculationType.PercentageAdd) {
                    offset = Mathf.RoundToInt(hpMaxBase * hpMaxEV);
                    curHPMax += offset;
                } else if (hpMaxNCT == NumCalculationType.PercentageMul) {
                    offset = Mathf.RoundToInt(curHPMax * hpMaxEV);
                    curHPMax += offset;
                } else if (hpMaxNCT == NumCalculationType.AbsoluteAdd) {
                    offset = hpMaxEV;
                    curHPMax += offset;
                }
                attributeEffectModel.hpMaxOffset += offset;
                attributeCom.SetHPMax(curHPMax);
                TDLog.Log($"Buff 影响 HPMax --> 变化 {offset} => 当前 {attributeCom.HPMax}");
            }

            // Move Speed
            var moveSpeedEffectTimes = attributeEffectModel.moveSpeedEffectTimes;
            if (buff.triggerTimes <= moveSpeedEffectTimes) {
                var moveSpeedBase = attributeCom.MoveSpeedBase;
                var curMoveSpeed = attributeCom.MoveSpeed;
                var finalMoveSpeed = curMoveSpeed;
                var offset = 0f;

                var moveSpeedEV = attributeEffectModel.moveSpeedEV;
                var moveSpeedNCT = attributeEffectModel.moveSpeedNCT;
                if (moveSpeedNCT == NumCalculationType.PercentageAdd) {
                    offset = Mathf.RoundToInt(moveSpeedBase * (moveSpeedEV));
                    finalMoveSpeed += offset;
                } else if (moveSpeedNCT == NumCalculationType.PercentageMul) {
                    offset = Mathf.RoundToInt(curMoveSpeed * (moveSpeedEV));
                    finalMoveSpeed += offset;
                } else if (moveSpeedNCT == NumCalculationType.AbsoluteAdd) {
                    offset = moveSpeedEV;
                    finalMoveSpeed += offset;
                }
                attributeEffectModel.moveSpeedOffset += offset;
                attributeCom.SetMoveSpeed(finalMoveSpeed);
                TDLog.Log($"Buff 影响 移动速度 --> 变化 {offset} => 当前 {attributeCom.MoveSpeed}");
            }

            // Normal Skill Speed
            var normalSkillSpeedBonusEffectTimes = attributeEffectModel.normalSkillSpeedBonusEffectTimes;
            if (buff.triggerTimes <= normalSkillSpeedBonusEffectTimes) {
                var ev = attributeEffectModel.normalSkillSpeedBonusEV;
                var offset = ev;
                var curBonus = attributeCom.NormalSkillSpeedBonus + offset;
                attributeEffectModel.normalSkillSpeedBonusOffset += offset;
                attributeCom.SetNormalSkillSpeedBonus(curBonus);
                TDLog.Log($"Buff 影响 普技加成 --> 变化 {offset} => 当前 {attributeCom.NormalSkillSpeedBonus}");
            }

            // Damage Bonus
            var physicalDamageBonusEffectTimes = attributeEffectModel.physicalDamageBonusEffectTimes;
            if (buff.triggerTimes <= physicalDamageBonusEffectTimes) {
                var ev = attributeEffectModel.physicalDamageBonusEV;
                var offset = ev;
                var curBonus = attributeCom.PhysicalDamageBonus + offset;
                attributeEffectModel.physicalDamageBonusOffset += offset;
                attributeCom.SetPhysicalDamageBonus(curBonus);
                TDLog.Log($"Buff 影响 物理伤害加成 --> 变化 {offset} => 当前 {attributeCom.PhysicalDamageBonus}");
            }
            var magicalDamageBonusEffectTimes = attributeEffectModel.magicalDamageBonusEffectTimes;
            if (buff.triggerTimes <= magicalDamageBonusEffectTimes) {
                var ev = attributeEffectModel.magicalDamageBonusEV;
                var offset = ev;
                var curBonus = attributeCom.MagicalDamageBonus + offset;
                attributeEffectModel.magicalDamageBonusOffset += offset;
                attributeCom.SetmagicalDamageBonus(curBonus);
                TDLog.Log($"Buff 影响 魔法伤害加成 --> 变化 {offset} => 当前 {attributeCom.MagicalDamageBonus}");
            }

            //  Defence Bonus
            var physicalDefenseBonusEffectTimes = attributeEffectModel.physicalDefenseBonusEffectTimes;
            if (buff.triggerTimes <= physicalDefenseBonusEffectTimes) {
                var ev = attributeEffectModel.physicalDefenseBonusEV;
                var offset = ev;
                var curBonus = attributeCom.PhysicalDefenseBonus + offset;
                attributeEffectModel.physicalDefenseBonusOffset += offset;
                attributeCom.SetPhysicalDefenseBonus(curBonus);
                TDLog.Log($"Buff 影响 物理减伤 --> 变化 {offset} => 当前 {attributeCom.PhysicalDefenseBonus}");
            }
            var magicalDefenseBonusEffectTimes = attributeEffectModel.magicalDefenseBonusEffectTimes;
            if (buff.triggerTimes <= magicalDefenseBonusEffectTimes) {
                var ev = attributeEffectModel.magicalDefenseBonusEV;
                var offset = ev;
                var curBonus = attributeCom.MagicalDefenseBonus + offset;
                attributeEffectModel.magicalDefenseBonusOffset += offset;
                attributeCom.SetMagicalDefenseBonus(curBonus);
                TDLog.Log($"Buff 影响 魔法减伤 --> 变化 {offset} => 当前 {attributeCom.MagicalDefenseBonus}");
            }

            return true;
        }

    }

}
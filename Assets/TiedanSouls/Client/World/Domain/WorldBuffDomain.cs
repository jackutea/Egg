using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

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

        #region [生成]

        /// <summary>
        /// 根据类型ID生成Buff
        /// </summary>
        public bool TrySpawn(int typeID, in EntityIDArgs father, out BuffEntity buff) {
            buff = null;

            var buffRepo = worldContext.BuffRepo;
            bool isFromPool = buffRepo.TryGetFromPool(typeID, out buff);
            if (!isFromPool) {
                var factory = worldContext.WorldFactory;
                if (!factory.TryCreateBuff(typeID, out buff)) {
                    TDLog.Error($"实体生成<Buff>失败 类型ID:{typeID}");
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
        /// 根据实体生成模型 生成Buff
        /// </summary>
        public bool TrySpawn_BySpawnModel(int fromFieldTypeID, in EntitySpawnModel entitySpawnModel, out BuffEntity buff) {
            buff = null;

            var typeID = entitySpawnModel.typeID;
            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateBuff(typeID, out buff)) {
                TDLog.Error($"生成Buff失败 typeID:{typeID}");
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

        #endregion

        #region [撤销]

        /// <summary>
        /// 撤销对属性值的影响,并回收Buff
        /// </summary>
        public void RevokeBuffFromAttribute(int buffEntityID, AttributeComponent attributeComponent) {
            var repo = worldContext.BuffRepo;
            if (repo.TryRemove(buffEntityID, out var buff)) {
                // - 撤销影响
                var attributeEffectModel = buff.AttributeEffectModel;
                if (attributeEffectModel.needRevoke_HPEV) {
                }
                if (attributeEffectModel.needRevoke_HPMaxEV) {
                }
                if (attributeEffectModel.needRevokePhysicsDamageBonusEV) {

                }
                if (attributeEffectModel.needRevokeMagicDamageBonus) {

                }
                // - 放回池子
                repo.AddToPool(buff);
            }
        }

        #endregion

        #region [效果器触发]

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
                    summonPos = role.LogicPos;
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
            var curHPMax = attributeCom.HPMax;

            // - HP
            var hpEV = attributeEffectModel.hpEV;
            var hpNCT = attributeEffectModel.hpNCT;
            var hpEffectTimes = attributeEffectModel.hpEffectTimes;
            if (hpNCT != NumCalculationType.None && buff.triggerTimes <= hpEffectTimes) {
                var curHP = attributeCom.HP;
                if (hpNCT == NumCalculationType.PercentageAdd) {
                    curHP += Mathf.RoundToInt(curHPMax * (hpEV / 100f));
                } else if (hpNCT == NumCalculationType.PercentageMul) {
                    curHP += Mathf.RoundToInt(curHP * (hpEV / 100f));
                } else if (hpNCT == NumCalculationType.AbsoluteAdd) {
                    curHP += hpEV;
                }
                attributeCom.SetHP(curHP);
                TDLog.Log($"Buff 影响 属性 --> HP: {curHP}");
            }

            // - HPMax
            var hpMaxEV = attributeEffectModel.hpMaxEV;
            var hpMaxNCT = attributeEffectModel.hpMaxNCT;
            var hpMaxEffectTimes = attributeEffectModel.hpMaxEffectTimes;
            if (hpMaxNCT != NumCalculationType.None && buff.triggerTimes <= hpMaxEffectTimes) {
                if (hpMaxNCT == NumCalculationType.PercentageAdd) {
                    var hpMaxBase = attributeCom.HPMaxBase;
                    curHPMax += Mathf.RoundToInt(hpMaxBase * (hpMaxEV / 100f));
                } else if (hpMaxNCT == NumCalculationType.PercentageMul) {
                    curHPMax += Mathf.RoundToInt(curHPMax * (hpMaxEV / 100f));
                } else if (hpMaxNCT == NumCalculationType.AbsoluteAdd) {
                    curHPMax += hpMaxEV;
                }
                attributeCom.SetHPMax(curHPMax);
                TDLog.Log($"Buff 影响 属性 --> HPMax: {curHPMax}");
            }

            // - Physics Damage Bonus
            var physicsDamageBonusEffectTimes = attributeEffectModel.physicsDamageBonusEffectTimes;
            if (buff.triggerTimes <= physicsDamageBonusEffectTimes) {
                var physicsDamageBonusEV = attributeEffectModel.physicsDamageBonusEV;
                attributeCom.AddPhysicalDamageBonus(physicsDamageBonusEV / 100f);
                TDLog.Log($"Buff 影响 属性 --> Physics Damage Bonus Increase: {physicsDamageBonusEV}");
            }

            // - Physics Defence Bonus
            var physicsDefenseBonusEffectTimes = attributeEffectModel.physicsDefenseBonusEffectTimes;
            if (buff.triggerTimes <= physicsDefenseBonusEffectTimes) {
                var magicDamageBonusEV = attributeEffectModel.magicDamageBonusEV;
                attributeCom.AddMagicDamageBonus(magicDamageBonusEV / 100f);
                TDLog.Log($"Buff 影响 属性 --> Physics Damage Bonus Increase: {magicDamageBonusEV}");
            }

            // - Magic Damage Bonus
            var magicDamageBonusEffectTimes = attributeEffectModel.magicDamageBonusEffectTimes;
            if (buff.triggerTimes <= magicDamageBonusEffectTimes) {
                var magicDamageBonusEV = attributeEffectModel.magicDamageBonusEV;
                attributeCom.AddMagicDamageBonus(magicDamageBonusEV / 100f);
                TDLog.Log($"Buff 影响 属性 --> Magic Damage Bonus Increase: {magicDamageBonusEV}");
            }

            // - Magic Defence Bonus
            var magicDefenseBonusEffectTimes = attributeEffectModel.magicDefenseBonusEffectTimes;
            if (buff.triggerTimes <= magicDefenseBonusEffectTimes) {
                var magicDefenseBonusEV = attributeEffectModel.magicDefenseBonusEV;
                attributeCom.AddMagicDefenseBonus(magicDefenseBonusEV / 100f);
                TDLog.Log($"Buff 影响 属性 --> Magic Defence Bonus Increase: {magicDefenseBonusEV}");
            }

            return true;
        }

        #endregion

    }

}
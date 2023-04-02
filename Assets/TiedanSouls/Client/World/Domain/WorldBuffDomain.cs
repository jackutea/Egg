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
        /// 撤销对角色属性值的影响,并回收Buff
        /// </summary>
        public void RevokeBuffFromRoleAttribute(int buffEntityID, RoleAttributeComponent attributeComponent) {
            var repo = worldContext.BuffRepo;
            if (repo.TryRemove(buffEntityID, out var buff)) {
                // - 撤销影响
                var attributeEffectModel = buff.RoleAttributeEffectModel;
                if (attributeEffectModel.needRevoke_HPEV) {
                }
                if (attributeEffectModel.needRevoke_HPMaxEV) {
                }
                // - 放回池子
                repo.AddToPool(buff);
            }
        }

        public void RevokeBuffFromWeaponttribute(int buffEntityID, WeaponAttributeComponent attributeComponent) {
            var repo = worldContext.BuffRepo;
            if (repo.TryRemove(buffEntityID, out var buff)) {
                // - 撤销影响
                var attributeEffectModel = buff.WeaponAttributeEffectModel;
                if (attributeEffectModel.needRevokePhysicsDamageIncreaseEV) {

                }
                if (attributeEffectModel.needRevokeMagicDamageIncrease) {

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

        public bool TryEffectRoleAttribute(RoleAttributeComponent attributeCom, BuffEntity buff) {
            if (!buff.IsTriggerFrame()) {
                return false;
            }

            var attributeEffectModel = buff.RoleAttributeEffectModel;
            var curHPMax = attributeCom.HPMax;

            // - HP
            var hpEV = attributeEffectModel.hpEV;
            var hpNCT = attributeEffectModel.hpNCT;
            if (hpNCT != NumCalculationType.None) {
                var curHP = attributeCom.HP;
                if (hpNCT == NumCalculationType.PercentageAdd) {
                    curHP += Mathf.RoundToInt(curHPMax * (hpEV / 100f));
                } else if (hpNCT == NumCalculationType.PercentageMul) {
                    curHP += Mathf.RoundToInt(curHP * (hpEV / 100f));
                } else if (hpNCT == NumCalculationType.AbsoluteAdd) {
                    curHP += hpEV;
                }
                attributeCom.SetHP(curHP);
                TDLog.Log($"Buff 影响 角色属性 --> HP: {curHP}");
            }

            // - HPMax
            var hpMaxEV = attributeEffectModel.hpMaxEV;
            var hpMaxNCT = attributeEffectModel.hpMaxNCT;
            if (hpMaxNCT != NumCalculationType.None) {
                if (hpMaxNCT == NumCalculationType.PercentageAdd) {
                    var hpMaxBase = attributeCom.HPMaxBase;
                    curHPMax += Mathf.RoundToInt(hpMaxBase * (hpMaxEV / 100f));
                } else if (hpMaxNCT == NumCalculationType.PercentageMul) {
                    curHPMax += Mathf.RoundToInt(curHPMax * (hpMaxEV / 100f));
                } else if (hpMaxNCT == NumCalculationType.AbsoluteAdd) {
                    curHPMax += hpMaxEV;
                }
                attributeCom.SetHPMax(curHPMax);
                TDLog.Log($"Buff 影响 角色属性 --> HPMax: {curHPMax}");
            }

            return true;
        }

        public bool TryEffectWeaponAttribute(WeaponAttributeComponent weaponAttributeCom, BuffEntity buff) {
            if (!buff.IsTriggerFrame()) {
                return false;
            }

            var weaponAttributeEffectModel = buff.WeaponAttributeEffectModel;

            // - Physics Damage Increase
            var physicsDamageIncreaseEV = weaponAttributeEffectModel.physicsDamageIncreaseEV;
            weaponAttributeCom.AddPhysicalDamageIncrease(physicsDamageIncreaseEV);

            // - Magic Damage Increase
            var magicDamageIncreaseEV = weaponAttributeEffectModel.magicDamageIncreaseEV;
            weaponAttributeCom.AddMagicDamageIncrease(magicDamageIncreaseEV);

            return true;
        }

        #endregion

    }

}
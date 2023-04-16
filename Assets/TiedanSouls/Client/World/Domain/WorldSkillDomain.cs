using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldSkillDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldSkillDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain rootDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = rootDomain;
        }

        #region [添加技能]

        public void AddAllSkillToSlot_Origin(SkillSlotComponent skillSlotCom, int[] typeIDArray, in EntityIDArgs father) {
            var templateCore = infraContext.TemplateCore;
            var idService = worldContext.IDService;
            var factory = worldContext.Factory;

            var len = typeIDArray.Length;
            for (int i = 0; i < len; i++) {
                var typeID = typeIDArray[i];
                if (!templateCore.SkillTemplate.TryGet(typeID, out SkillTM skillTM)) {
                    continue;
                }

                if (!factory.TryCreateSkillEntity(skillTM.typeID, out SkillEntity skillEntity)) {
                    continue;
                }

                var idCom = skillEntity.IDCom;
                idCom.SetEntityID(idService.PickSkillID());

                if (!skillSlotCom.TryAdd_Origin(skillEntity)) {
                    TDLog.Error($"添加技能失败! 已添加 '原始' 技能 - {typeID}");
                }

                skillEntity.IDCom.SetFather(father);
                var skillRepo = worldContext.SkillRepo;
                if (!skillRepo.TryAdd(skillEntity)) {
                    TDLog.Error($"Repo已存在该技能! - {skillEntity.IDCom}");
                } else {
                    TDLog.Log($"添加技能成功! 已添加 '原始' 技能 - {typeID}");
                }
            }

            // 技能碰撞盒关联
            skillSlotCom.Foreach_Origin((skill) => {
                var skillIDArgs = skill.IDCom.ToArgs();
                rootDomain.SetEntityColliderTriggerModelFathers(skill.EntityColliderTriggerModelArray, skillIDArgs);
            });
        }

        public void AddAllSkillToSlot_Combo(SkillSlotComponent skillSlotCom, in EntityIDArgs father) {
            EntityIDArgs father_lambda = father;
            skillSlotCom.Foreach_Origin((skill) => {
                var cancelModelArray = skill.ComboSkillCancelModelArray;
                AddComboSkill(skillSlotCom, cancelModelArray, father_lambda);
            });

            void AddComboSkill(SkillSlotComponent skillSlotCom, SkillCancelModel[] cancelModelArray, in EntityIDArgs father) {
                var len = cancelModelArray?.Length;
                for (int i = 0; i < len; i++) {
                    var cancelModel = cancelModelArray[i];
                    var comboTypeID = cancelModel.skillTypeID;
                    if (!worldContext.Factory.TryCreateSkillEntity(comboTypeID, out SkillEntity comboSkill)) {
                        continue;
                    }

                    var idCom = comboSkill.IDCom;
                    idCom.SetEntityID(worldContext.IDService.PickSkillID());

                    if (!skillSlotCom.TryAdd_Combo(comboSkill)) {
                        TDLog.Error($"添加技能失败! - {comboTypeID}");
                        continue;
                    }

                    idCom.SetFather(father);
                    var skillRepo = worldContext.SkillRepo;
                    if (!skillRepo.TryAdd(comboSkill)) {
                        TDLog.Error($"Repo已存在该技能! - {idCom}");
                    } else {
                        TDLog.Log($"添加技能成功! 已添加 '组合技' 技能 - {comboTypeID}");
                        AddComboSkill(skillSlotCom, comboSkill.ComboSkillCancelModelArray, father_lambda);
                    }
                }

                // 技能碰撞盒关联
                skillSlotCom.Foreach_Combo((skill) => {
                    var skillIDArgs = skill.IDCom.ToArgs();
                    rootDomain.SetEntityColliderTriggerModelFathers(skill.EntityColliderTriggerModelArray, skillIDArgs);
                });
            }
        }

        #endregion

        #region [击中 & 受击]

        /// <summary>
        /// 技能 击中 处理
        /// </summary>
        public void HandleHit(SkillEntity skill, Vector3 skillColliderPos) {
            if (!skill.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            var hitEffectorTypeID = collisionTriggerModel.hitEffectorTypeID;
            var effectorDomain = rootDomain.EffectorDomain;
            if (!effectorDomain.TrySpawnEffectorModel(hitEffectorTypeID, out var effectorModel)) {
                return;
            }

            var summoner = skill.IDCom.ToArgs();
            var entityDestroyModelArray = effectorModel.entityDestroyModelArray;
            this.rootDomain.DestroyBy_EntityDestroyModelArray(summoner, entityDestroyModelArray);
            TDLog.Log($"击中效果 - {hitEffectorTypeID}");
        }

        /// <summary>
        /// 技能 受击 处理
        /// </summary>
        public void HandleBeHit(SkillEntity skill, in EntityColliderTriggerModel collisionTriggerModel, int hitFrame) {

        }

        #endregion

    }

}
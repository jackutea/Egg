using UnityEngine;
using GameArki.BTTreeNS;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Client.Facades;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    public class WorldFactory {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldFactory() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        #region [Field]

        public bool TryCreateFieldEntity(int typeID, out FieldEntity field) {
            field = null;

            var fieldTemplate = infraContext.TemplateCore.FieldTemplate;
            if (!fieldTemplate.TryGet(typeID, out FieldTM fieldTM)) {
                TDLog.Error($"Failed to get field template: {typeID}");
                return false;
            }

            var fieldModAssetName = fieldTM.fieldAssetName;

            var fieldModAssets = infraContext.AssetCore.FieldModAsset;
            bool has = fieldModAssets.TryGet(fieldModAssetName, out GameObject go);
            if (!has) {
                TDLog.Error($"Failed to get asset: {fieldModAssetName}");
                return false;
            }

            field = new FieldEntity();

            var idCom = field.IDCom;
            var service = worldContext.IDService;
            idCom.SetEntityID(service.PickFieldID());
            idCom.SetTypeID(typeID);

            var fieldGO = GameObject.Instantiate(go);
            fieldGO.transform.position = Vector3.zero;
            field.SetFieldMod(fieldGO);

            field.SetEntitySpawnCtrlModelArray(GetEntitySpawnCtrlModelArray(fieldTM.entitySpawnCtrlTMArray));
            field.SetItemSpawnPosArray(fieldTM.itemSpawnPosArray?.Clone() as Vector2[]);
            field.SetFieldType(fieldTM.fieldType);
            field.SetFieldDoorArray(fieldTM.fieldDoorArray?.Clone() as FieldDoorModel[]);

            return true;
        }

        #endregion

        #region [Item]

        public bool TryCreateItemEntity(int typeID, out ItemEntity item) {
            item = null;

            var templateCore = infraContext.TemplateCore;

            // Template
            if (!templateCore.ItemTemplate.TryGet(typeID, out ItemTM itemTM)) {
                TDLog.Error($"配置出错! 未找到 物件 模板数据: TypeID {typeID}");
                return false;
            }

            // Check
            var itemType = itemTM.itemType;
            if (itemType == ItemType.None) {
                TDLog.Error("物件类型 is None");
                return false;
            }

            // Container
            var assetCore = infraContext.AssetCore;
            var containerModAssets = assetCore.ContainerModAsset;
            var contanerAssetName = "mod_container_item";
            if (!containerModAssets.TryGet(contanerAssetName, out GameObject itemPrefab)) {
                TDLog.Error($"获取实体容器失败! {contanerAssetName}");
                return false;
            }

            // ItemEntity
            var itemGo = GameObject.Instantiate(itemPrefab);
            item = itemGo.GetComponent<ItemEntity>();
            item.Ctor();

            var idCom = item.IDCom;
            var idService = worldContext.IDService;
            var itemID = idService.PickItemID();
            idCom.SetEntityID(itemID);
            idCom.SetTypeID(typeID);

            item.SetTypeIDForPickUp(itemTM.typeIDForPickUp);
            item.SetItemType(itemType);

            // Asset
            var itemAssetName = itemTM.itemAssetName;
            var itemModAssets = assetCore.ItemModAsset;
            if (!itemModAssets.TryGet(itemAssetName, out GameObject modPrefab)) {
                TDLog.Error($"Failed to get ModAsset: {itemAssetName}");
                return false;
            }

            // Set Mod
            var mod = GameObject.Instantiate(modPrefab);
            item.SetMod(mod);

            return item;
        }

        #endregion

        #region [Role]

        public bool TryCreateRoleEntity(int typeID, out RoleEntity role) {
            role = null;

            // TM
            var templateCore = infraContext.TemplateCore;
            if (!templateCore.RoleTemplate.TryGet(typeID, out RoleTM roleTM)) {
                TDLog.Error($"配置出错! 未找到角色模板数据: TypeID {typeID}");
                return false;
            }

            // Container
            var assetCore = infraContext.AssetCore;
            var containerModAssets = assetCore.ContainerModAsset;
            var contanerAssetName = "mod_container_role";
            bool has = containerModAssets.TryGet(contanerAssetName, out GameObject go);
            if (!has) {
                TDLog.Error($"获取实体容器失败! {contanerAssetName}");
                return false;
            }

            role = GameObject.Instantiate(go).GetComponent<RoleEntity>();
            role.Ctor();

            // ID
            var roleIDCom = role.IDCom;
            roleIDCom.SetTypeID(typeID);
            roleIDCom.SetEntityName(roleTM.roleName);

            // FSM
            var fsmCom = role.FSMCom;
            fsmCom.EnterActionState_Idle();

            // Mod
            var roleModAssets = assetCore.RoleModAsset;
            has = roleModAssets.TryGet(roleTM.modName, out GameObject roleModPrefab);
            if (!has) {
                TDLog.Error($"请检查配置! 角色模型资源不存在! {roleTM.modName}");
                return false;
            }
            GameObject roleMod = GameObject.Instantiate(roleModPrefab, role.RendererRoot);
            role.SetMod(roleMod);

            // Attribute
            var attrCom = role.AttributeCom;
            SetAttributeComponent(attrCom, roleTM);

            // HUD
            var hpBar = CreateHpBarHUD();
            role.HudSlotCom.SetHpBarHUD(hpBar);

            return role;
        }

        public RoleAIStrategy CreateAIStrategy(RoleEntity role, int typeID) {
            var templateCore = infraContext.TemplateCore;
            var assetCore = infraContext.AssetCore;

            // TM
            var has = templateCore.AITemplate.TryGet(typeID, out AITM aiTM);
            if (!has) {
                if (templateCore.AITemplate.TryGet(0, out AITM neutralTM)) {
                    aiTM = neutralTM;
                }
                TDLog.Warning("Failed to get AI template: " + typeID);
            }

            // Nodes
            var patrolAIPrecondition = new RolePatrolAIPrecondition(aiTM.sight);
            var followAIPrecondition = new RoleFollowAIPrecondition(aiTM.atkRange);
            var followAIAction = new RoleFollowAIAction(aiTM.atkRange, aiTM.sight);
            var patrolAIAction = new RolePatrolAIAction(aiTM.sight);
            var attackAIAction = new RoleAttackAIAction(aiTM.atkRange, aiTM.atkCD);

            patrolAIPrecondition.Inject(role, worldContext);
            followAIPrecondition.Inject(role, worldContext);
            followAIAction.Inject(role, worldContext);
            patrolAIAction.Inject(role, worldContext);
            attackAIAction.Inject(role, worldContext);

            BTTreeNode patrolNode = BTTreeFactory.CreateActionNode(patrolAIAction, patrolAIPrecondition);
            BTTreeNode followNode = BTTreeFactory.CreateActionNode(followAIAction, followAIPrecondition);
            BTTreeNode attackNode = BTTreeFactory.CreateActionNode(attackAIAction);
            BTTreeNode followRoot = BTTreeFactory.CreateSelectorNode();
            BTTreeNode root = BTTreeFactory.CreateSelectorNode();

            followRoot.AddChild(followNode);
            if (role.IDCom.AllyStatus == AllyType.Two) followRoot.AddChild(attackNode);
            root.AddChild(patrolNode);
            root.AddChild(followRoot);

            // Tree
            BTTree bt = new BTTree();
            bt.Initialize(root);

            // Strategy
            RoleAIStrategy ai = new RoleAIStrategy();
            ai.Inject(bt);

            return ai;

        }

        public HpBarHUD CreateHpBarHUD() {
            var assetCore = infraContext.AssetCore;
            var hudAssets = assetCore.HUDAsset;
            bool has = hudAssets.TryGet("hud_hp_bar", out GameObject go);
            if (!has) {
                TDLog.Error("Failed to get asset: hud_hp_bar");
                return null;
            }
            var hud = GameObject.Instantiate(go).GetComponent<HpBarHUD>();
            hud.Ctor();
            return hud;
        }

        #endregion

        #region [Skill]

        public bool TryCreateSkillEntity(int skillTypeID, out SkillEntity skill) {
            skill = null;

            var skillTemplate = infraContext.TemplateCore.SkillTemplate;
            if (!skillTemplate.TryGet(skillTypeID, out SkillTM skillTM)) {
                TDLog.Error($"配置出错! 未找到技能模板数据: TypeID {skillTypeID}");
                return false;
            }

            skill = new SkillEntity();

            // 基础信息
            var idCom = skill.IDCom;
            idCom.SetTypeID(skillTypeID);
            idCom.SetEntityName(skillTM.skillName);
            skill.SetMaintainFrame(skillTM.maintainFrame);
            // 技能类型
            skill.SetSkillType(skillTM.skillType);
            // 原始技能类型
            skill.SetOriginalSkillTypeID(skillTM.originSkillTypeID);
            // 组合技能清单
            skill.SetComboSkillCancelModelArray(TM2ModelUtil.GetSkillCancelModelArray(skillTM.comboSkillCancelTMArray));
            // 连招技能清单
            skill.SetLinkSkillCancelModelArray(TM2ModelUtil.GetSkillCancelModelArray(skillTM.cancelSkillCancelTMArray));
            // 碰撞器
            skill.SetEntityColliderTriggerModelArray(TM2ModelUtil.GetEntityColliderTriggerModelArray(skillTM.collisionTriggerTMArray));
            // 技能效果器
            skill.SetSkillEffectorModelArray(TM2ModelUtil.GetSkillEffectorModelArray(skillTM.skillEffectorTMArray));
            // 技能位移曲线
            skill.SetSkillMoveCurveModelArray(TM2ModelUtil.GetSkillMoveCurveModelArray(skillTM.skillMoveCurveTMArray));
            // 武器动画
            skill.SetWeaponAnimName(skillTM.weaponAnimName);

            return true;
        }

        #endregion

        #region [Projectile]

        public bool TryCreateProjectile(int typeID, out ProjectileEntity projectile) {
            projectile = null;

            var template = infraContext.TemplateCore.ProjectileTemplate;
            if (!template.TryGet(typeID, out ProjectileTM projetileTM)) {
                TDLog.Error($"配置出错! 未找到 弹道 模板数据: TypeID {typeID}");
                return false;
            }

            projectile = new ProjectileEntity();

            // ID
            var idCom = projectile.IDCom;
            idCom.SetTypeID(typeID);
            idCom.SetEntityName(projetileTM.projectileName);

            // 弹道子弹模型数据
            var projetileBulletTMArray = projetileTM.projetileBulletTMArray;
            if (projetileBulletTMArray != null) {
                var len = projetileBulletTMArray.Length;
                ProjectileBulletModel[] projetileBulletModelArray = new ProjectileBulletModel[len];
                for (int i = 0; i < len; i++) {
                    var tm = projetileBulletTMArray[i];
                    var model = TM2ModelUtil.GetProjectileBulletModel(tm);
                    projetileBulletModelArray[i] = model;
                }
                projectile.SetProjectileBulletModelArray(projetileBulletModelArray);
            }

            return true;
        }

        #endregion

        #region [Bullet]

        public bool TryCreateBullet(int bulletTypeID, out BulletEntity bullet) {
            bullet = null;

            var templateCore = infraContext.TemplateCore;
            var bulletTemplate = templateCore.BulletTemplate;
            if (!bulletTemplate.TryGet(bulletTypeID, out BulletTM tm)) {
                TDLog.Error($"配置出错! 未找到 子弹 模板数据: TypeID {bulletTypeID}");
                return false;
            }

            bullet = new BulletEntity();
            bullet.Ctor();

            var idCom = bullet.IDCom;
            idCom.SetTypeID(tm.typeID);
            idCom.SetEntityName(tm.bulletName);

            bullet.SetMaintainFrame(tm.maintainFrame);

            bullet.SetTrajectoryType(tm.trajectoryType);
            bullet.entityTrackModel = TM2ModelUtil.GetEntityTrackModel(tm.entityTrackTM);// 实体追踪模型
            bullet.moveCurveModel = TM2ModelUtil.GetMoveCurveModel(tm.moveCurveTM);// 位移曲线模型

            bullet.SetCollisionTriggerModel(TM2ModelUtil.GetEntityColliderTriggerModel(tm.collisionTriggerTM));

            bullet.SetDeathEffectorTypeID(tm.deathEffectorTypeID);

            bullet.SetExtraPenetrateCount(tm.extraPenetrateCount);

            var assetCore = infraContext.AssetCore;
            var vfxAsset = assetCore.VFXAsset;
            if (!vfxAsset.TryGet(tm.vfxPrefabName, out GameObject vfxGO)) {
                TDLog.Error($"获取 VFX 失败! {tm.vfxPrefabName}");
                return false;
            }
            bullet.SetVFXGO(GameObject.Instantiate(vfxGO));

            TDLog.Log($"创建子弹 {bulletTypeID}");
            return true;
        }

        #endregion

        #region [Buff]

        public bool TryCreateBuff(int buffTypeID, out BuffEntity buff) {
            buff = null;

            var templateCore = infraContext.TemplateCore;
            var buffTemplate = templateCore.BuffTemplate;
            if (!buffTemplate.TryGet(buffTypeID, out BuffTM tm)) {
                TDLog.Error($"配置出错! 未找到 Buff 模板数据: TypeID {buffTypeID}");
                return false;
            }

            buff = new BuffEntity();
            buff.Ctor();

            var idCom = buff.IDCom;
            idCom.SetTypeID(tm.typeID);
            idCom.SetEntityName(tm.buffName);

            buff.SetDescription(tm.description);
            buff.SetIconName(tm.iconName);

            buff.SetDelayFrame(tm.delayFrame);
            buff.SetIntervalFrame(tm.intervalFrame);
            buff.SetDurationFrame(tm.durationFrame);

            buff.SetAttributeEffectModel(TM2ModelUtil.GetAttributeEffectModel(tm.attributeEffectTM));
            buff.SetEffectorTypeID(tm.effectorTypeID);

            return true;
        }

        #endregion

        #region [EntitySpawnCtrl]

        public EntitySpawnCtrlModel[] GetEntitySpawnCtrlModelArray(EntitySpawnCtrlTM[] tmArray) {
            var len = tmArray.Length;
            var modelArray = new EntitySpawnCtrlModel[len];
            for (int i = 0; i < len; i++) {
                var tm = tmArray[i];
                var model = GetEntitySpawnCtrlModel(tm);
                modelArray[i] = model;
            }
            return modelArray;
        }

        public EntitySpawnCtrlModel GetEntitySpawnCtrlModel(EntitySpawnCtrlTM tm) {
            EntitySpawnCtrlModel model;
            model.spawnFrame = tm.spawnFrame;
            model.isBreakPoint = tm.isBreakPoint;
            model.entitySpawnModel = GetEntitySpawnModel(tm.entitySpawnTM);
            return model;
        }

        #endregion

        #region [EntitySpawn]

        public EntitySpawnModel GetEntitySpawnModel(EntitySpawnTM tm) {
            EntitySpawnModel model;
            model.entityType = tm.entityType;
            model.typeID = tm.typeID;
            model.controlType = tm.controlType;
            model.allyType = tm.allyType;
            model.spawnPos = tm.spawnPos;
            model.isBoss = tm.isBoss;
            return model;
        }

        #endregion

        #region [Component]

        public static void SetAttributeComponent(AttributeComponent attributeComponent, RoleTM tm) {
            // TODO: 天赋系统的初始属性加成
            var hpMax_base = TM2ModelUtil.GetFloat_Shrink100(tm.hpMax_Expanded);
            var mpMax_base = TM2ModelUtil.GetFloat_Shrink100(tm.mpMax_Expanded);
            var gpMax_base = TM2ModelUtil.GetFloat_Shrink100(tm.gpMax_Expanded);
            var moveSpeed_base = TM2ModelUtil.GetFloat_Shrink100(tm.moveSpeed_Expanded);
            var jumpSpeed_base = TM2ModelUtil.GetFloat_Shrink100(tm.jumpSpeed_Expanded);

            attributeComponent.SetHPMaxBase(hpMax_base);
            attributeComponent.SetHPMax(hpMax_base);
            attributeComponent.SetHP(hpMax_base);
            attributeComponent.SetMPMaxBase(mpMax_base);
            attributeComponent.SetMPMax(mpMax_base);
            attributeComponent.SetMP(mpMax_base);
            attributeComponent.SetGPMaxBase(gpMax_base);
            attributeComponent.SetGPMax(gpMax_base);
            attributeComponent.SetGP(gpMax_base);
            attributeComponent.SetMoveSpeedBase(moveSpeed_base);
            attributeComponent.SetMoveSpeed(moveSpeed_base);
            attributeComponent.SetJumpSpeedBase(jumpSpeed_base);
            attributeComponent.SetJumpSpeed(jumpSpeed_base);
            attributeComponent.SetFallSpeed(TM2ModelUtil.GetFloat_Shrink100(tm.fallSpeed_Expanded));
            attributeComponent.SetFallSpeedMax(TM2ModelUtil.GetFloat_Shrink100(tm.fallSpeedMax_Expanded));
        }

        #endregion

    }

}
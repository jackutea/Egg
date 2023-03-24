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

            field = GameObject.Instantiate(go).GetComponent<FieldEntity>();
            field.Ctor();

            var idCom = field.IDCom;
            var service = worldContext.IDService;
            idCom.SetEntityID(service.PickFieldID());
            idCom.SetTypeID(typeID);

            field.SetEntitySpawnCtrlModelArray(GetModelArray_EntitySpawnCtrl(fieldTM.entitySpawnCtrlTMArray));
            field.SetItemSpawnPosArray(fieldTM.itemSpawnPosArray?.Clone() as Vector2[]);
            field.SetFieldType(fieldTM.fieldType);
            field.SetFieldDoorArray(fieldTM.fieldDoorArray?.Clone() as FieldDoorModel[]);

            return field;
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

            // Mod
            var roleModAssets = assetCore.RoleModAsset;
            has = roleModAssets.TryGet(roleTM.modName, out GameObject roleModPrefab);
            if (!has) {
                TDLog.Error($"请检查配置! 角色模型资源不存在! {roleTM.modName}");
                return false;
            }
            GameObject roleMod = GameObject.Instantiate(roleModPrefab, role.RendererRoot);
            role.SetMod(roleMod);

            // Attr
            var attrCom = role.AttributeCom;
            attrCom.InitializeHealth(roleTM.hpMax, roleTM.hpMax, roleTM.epMax, roleTM.epMax, roleTM.gpMax, roleTM.gpMax);
            attrCom.InitializeLocomotion(roleTM.moveSpeed, roleTM.jumpSpeed, roleTM.fallingAcceleration, roleTM.fallingSpeedMax);

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
            if (role.IDCom.AllyType == AllyType.Two) followRoot.AddChild(attackNode);
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
            skill.SetComboSkillCancelModelArray(TM2ModelUtil.GetModelArray_SkillCancel(skillTM.comboSkillCancelTMArray));
            // 连招技能清单
            skill.SetLinkSkillCancelModelArray(TM2ModelUtil.GetModelArray_SkillCancel(skillTM.cancelSkillCancelTMArray));
            // 碰撞器
            skill.SetCollisionTriggerArray(TM2ModelUtil.GetModelArray_CollisionTrigger(skillTM.collisionTriggerTMArray));
            // 技能效果器
            skill.SetSkillEffectorModelArray(TM2ModelUtil.GetModelArray_SkillEffector(skillTM.skillEffectorTMArray));
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
            projectile.Ctor();

            // ID
            var idCom = projectile.IDCom;
            idCom.SetTypeID(typeID);
            idCom.SetEntityName(projetileTM.projectileName);

            // 弹道子弹
            var projetileBulletTMArray = projetileTM.projetileBulletTMArray;
            var len = projetileBulletTMArray.Length;
            ProjectileBulletModel[] projectileBulletModelArray = new ProjectileBulletModel[len];
            for (int i = 0; i < len; i++) {
                var projetileBulletTM = projetileBulletTMArray[i];
                if (!TryGetProjectileBulletModel(projetileBulletTM, out ProjectileBulletModel projectileBulletModel)) {
                    TDLog.Error($"创建 弹道子弹 失败! {projetileBulletTM.bulletTypeID}");
                    return false;
                }

                projectileBulletModelArray[i] = projectileBulletModel;
            }
            projectile.SetProjectileBulletModelArray(projectileBulletModelArray);

            return true;
        }

        #endregion

        #region [ProjectileBullet]

        public bool TryGetProjectileBulletModel(ProjectileBulletTM tm, out ProjectileBulletModel model) {
            model.extraHitTimes = tm.extraHitTimes;
            model.localPos = tm.localPos;
            model.localEulerAngles = tm.localEulerAngles;
            if (!TryCreateBullet(tm.bulletTypeID, out model.bulletEntity)) {
                TDLog.Error($"创建子弹失败! {tm.bulletTypeID}");
                return false;
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

            bullet.SetCollisionTriggerModel(TM2ModelUtil.GetModel_CollisionTrigger(tm.collisionTriggerTM));
            bullet.SetHitEffectorModel(TM2ModelUtil.GetModel_Effector(tm.hitEffectorTM));
            bullet.SetDeathEffectorModel(TM2ModelUtil.GetModel_Effector(tm.deathEffectorTM));

            bullet.SetMoveSpeedArray(TM2ModelUtil.GetFloatArray_Shrink100(tm.moveSpeedArray_cm));
            bullet.SetDirectionArray(tm.directionArray?.Clone() as Vector3[]);

            // 表现层
            var assetCore = infraContext.AssetCore;
            var vfxAsset = assetCore.VFXAsset;
            if (!vfxAsset.TryGet(tm.vfxPrefabName, out GameObject vfxGO)) {
                TDLog.Error($"获取 VFX 失败! {tm.vfxPrefabName}");
                return false;
            }
            bullet.SetVFXGO(vfxGO);

            return true;
        }

        #endregion

        #region [EntitySpawnCtrl]

        public EntitySpawnCtrlModel[] GetModelArray_EntitySpawnCtrl(EntitySpawnCtrlTM[] tmArray) {
            var len = tmArray.Length;
            var modelArray = new EntitySpawnCtrlModel[len];
            for (int i = 0; i < len; i++) {
                var tm = tmArray[i];
                var model = GetModel_EntitySpawnCtrl(tm);
                modelArray[i] = model;
            }
            return modelArray;
        }

        public EntitySpawnCtrlModel GetModel_EntitySpawnCtrl(EntitySpawnCtrlTM tm) {
            EntitySpawnCtrlModel model;
            model.spawnFrame = tm.spawnFrame;
            model.isBreakPoint = tm.isBreakPoint;
            model.entitySpawnModel = GetModel_EntitySpawn(tm.entitySpawnTM);
            return model;
        }

        #region [EntitySpawn]

        public EntitySpawnModel GetModel_EntitySpawn(EntitySpawnTM tm) {
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

        #endregion

    }

}
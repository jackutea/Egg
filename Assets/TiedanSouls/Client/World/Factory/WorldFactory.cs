using UnityEngine;
using GameArki.BTTreeNS;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Entities;
using TiedanSouls.World.Facades;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.World {

    public class WorldFactory {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldFactory() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        #region [Field]

        public bool TrySpawnFieldEntity(int typeID, out FieldEntity field) {
            field = null;

            var fieldTemplate = infraContext.TemplateCore.FieldTemplate;
            if (!fieldTemplate.TryGet(typeID, out FieldTM fieldTM)) {
                TDLog.Error($"Failed to get field template: {typeID}");
                return false;
            }

            var fieldModAssetName = fieldTM.fieldAssetName;

            var fieldModAssets = infraContext.AssetCore.FieldModAssets;
            bool has = fieldModAssets.TryGet(fieldModAssetName, out GameObject go);
            if (!has) {
                TDLog.Error($"Failed to get asset: {fieldModAssetName}");
                return false;
            }

            field = GameObject.Instantiate(go).GetComponent<FieldEntity>();
            field.Ctor();

            field.SetTypeID(typeID);
            field.SetSpawnModelArray(fieldTM.spawnModelArray?.Clone() as SpawnModel[]);
            field.SetItemSpawnPosArray(fieldTM.itemSpawnPosArray?.Clone() as Vector2[]);
            field.SetFieldType(fieldTM.fieldType);
            field.SetFieldDoorArray(fieldTM.fieldDoorArray?.Clone() as FieldDoorModel[]);

            return field;
        }

        #endregion

        #region [Role]

        public RoleEntity SpawnRoleEntity(ControlType controlType, int typeID, AllyType allyType, Vector2 bornPos) {
            var idService = worldContext.IDService;
            var templateCore = infraContext.TemplateCore;
            var assetCore = infraContext.AssetCore;

            // Container
            var containerModAssets = assetCore.ContainerModAssets;
            var contanerAssetName = "mod_container_role";
            bool has = containerModAssets.TryGet(contanerAssetName, out GameObject go);
            if (!has) {
                TDLog.Error($"Failed to get asset: {contanerAssetName}");
                return null;
            }

            var role = GameObject.Instantiate(go).GetComponent<RoleEntity>();
            role.Ctor();
            role.SetBornPos(bornPos);

            // ID
            int id = idService.PickRoleID();
            role.SetEntityD(id);
            role.SetTypeID(typeID);

            // TM
            has = templateCore.RoleTemplate.TryGet(typeID, out RoleTM roleTM);
            if (!has) {
                TDLog.Error("Failed to get role template: " + typeID);
                return null;
            }

            // Role Name
            role.SetRoleName(roleTM.roleName);

            // Mod
            var roleModAssets = assetCore.RoleModAssets;
            has = roleModAssets.TryGet(roleTM.modName, out GameObject roleModPrefab);
            if (!has) {
                TDLog.Error($"请检查配置! 角色模型资源不存在! {roleTM.modName}");
                return null;
            }
            GameObject roleMod = GameObject.Instantiate(roleModPrefab, role.RendererRoot);
            role.SetMod(roleMod);

            // Attr
            var attrCom = role.AttrCom;
            attrCom.InitializeHealth(roleTM.hpMax, roleTM.hpMax, roleTM.epMax, roleTM.epMax, roleTM.gpMax, roleTM.gpMax);
            attrCom.InitializeLocomotion(roleTM.moveSpeed, roleTM.jumpSpeed, roleTM.fallingAcceleration, roleTM.fallingSpeedMax);

            // Pos
            role.SetPos_Logic(bornPos);
            role.SyncRendererr();

            // Ally
            role.SetAlly(allyType);

            // ControlType
            role.SetControlType(controlType);

            if (controlType == ControlType.AI) {
                var ai = CreateAIStrategy(role, typeID);
                role.SetAIStrategy(ai);
            }

            // HUD
            var hpBar = CreateHpBarHUD();
            role.HudSlotCom.SetHpBarHUD(hpBar);

            return role;
        }

        RoleAIStrategy CreateAIStrategy(RoleEntity role, int typeID) {

            // Nodes
            var patrolAIPrecondition = new RolePatrolAIPrecondition();
            var followAIPrecondition = new RoleFollowAIPrecondition();
            var followAIAction = new RoleFollowAIAction();
            var patrolAIAction = new RolePatrolAIAction();
            var attackAIAction = new RoleAttackAIAction();

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
            if (role.AllyType == AllyType.Enemy) followRoot.AddChild(attackNode);
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
            var hudAssets = assetCore.HUDAssets;
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

        #region [Item]

        public ItemEntity SpawnItemEntity(int itemTypeID, Vector2 pos, int fromFieldTypeID) {
            ItemEntity itemEntity = null;

            var templateCore = infraContext.TemplateCore;

            // Template
            if (!templateCore.ItemTemplate.TryGet(itemTypeID, out ItemTM itemTM)) {
                TDLog.Error("Failed to get Item template: " + itemTypeID);
                return null;
            }

            // Check
            var itemType = itemTM.itemType;
            if (itemType == ItemType.None) {
                TDLog.Error("ItemType is None");
                return null;
            }

            // Container
            var assetCore = infraContext.AssetCore;
            var containerModAssets = assetCore.ContainerModAssets;
            var contanerAssetName = "mod_container_item";
            if (!containerModAssets.TryGet(contanerAssetName, out GameObject itemPrefab)) {
                TDLog.Error($"Failed to get Container: {contanerAssetName}");
                return null;
            }

            // ItemEntity
            var itemGo = GameObject.Instantiate(itemPrefab);
            var idService = worldContext.IDService;
            var itemID = idService.PickItemID();
            itemEntity = itemGo.GetComponent<ItemEntity>();
            itemEntity.Ctor();
            itemEntity.SetEntityD(itemID);
            itemEntity.SetTypeID(itemTypeID);
            itemEntity.SetTypeIDForPickUp(itemTM.typeIDForPickUp);
            itemEntity.SetItemType(itemType);
            itemEntity.SetPos(pos);

            // Asset
            var itemAssetName = itemTM.itemAssetName;
            var itemModAssets = assetCore.ItemModAsset;
            if (!itemModAssets.TryGet(itemAssetName, out GameObject modPrefab)) {
                TDLog.Error($"Failed to get ModAsset: {itemAssetName}");
                return null;
            }

            // Set Mod
            var mod = GameObject.Instantiate(modPrefab);
            itemEntity.SetMod(mod);

            // Repo
            var itemRepo = worldContext.ItemRepo;
            itemRepo.Add(itemEntity, fromFieldTypeID);

            return itemEntity;
        }

        #endregion

    }

}
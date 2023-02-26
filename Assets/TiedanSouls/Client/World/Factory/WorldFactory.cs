using UnityEngine;
using GameArki.BTTreeNS;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Entities;
using TiedanSouls.World.Facades;
using TiedanSouls.Template;
using System.Collections.Generic;

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

        public FieldEntity SpawnFieldEntity(int typeID) {
            var fieldTemplate = infraContext.TemplateCore.FieldTemplate;
            if (!fieldTemplate.TryGet(typeID, out FieldTM fieldTM)) {
                TDLog.Error($"Failed to get field template: {typeID}");
                return null;
            }

            var fieldModAssetName = fieldTM.fieldAssetName;

            var fieldModAssets = infraContext.AssetCore.FieldModAssets;
            bool has = fieldModAssets.TryGet(fieldModAssetName, out GameObject go);
            if (!has) {
                TDLog.Error($"Failed to get asset: {fieldModAssetName}");
                return null;
            }

            var entity = GameObject.Instantiate(go).GetComponent<FieldEntity>();
            entity.Ctor();

            entity.SetChapterAndLevel(fieldTM.chapter, fieldTM.level);
            entity.SetSpawnModelArray(fieldTM.spawnModelArray?.Clone() as SpawnModel[]);
            entity.SetItemSpawnPosArray(fieldTM.itemSpawnPosArray?.Clone() as Vector2[]);
            entity.SetFieldType(fieldTM.fieldType);

            return entity;
        }

        #endregion

        #region [Role]

        public RoleEntity SpawnRoleEntity(RoleControlType controlType, int typeID, AllyType allyType, Vector2 pos) {
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

            // ID
            int id = idService.PickRoleID();
            role.SetID(id);

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
                TDLog.Error("Failed to get sprite: " + roleTM.modName);
                return null;
            }
            GameObject roleMod = GameObject.Instantiate(roleModPrefab, role.Body);
            role.SetMod(roleMod);

            // Attr
            var attrCom = role.AttrCom;
            attrCom.InitializeHealth(roleTM.hpMax, roleTM.hpMax, roleTM.epMax, roleTM.epMax, roleTM.gpMax, roleTM.gpMax);
            attrCom.InitializeLocomotion(roleTM.moveSpeed, roleTM.jumpSpeed, roleTM.fallingAcceleration, roleTM.fallingSpeedMax);

            // Pos
            role.SetPos(pos);

            // Ally
            role.SetAlly(allyType);

            // ControlType
            role.SetControlType(controlType);

            if (controlType == RoleControlType.AI) {
                var ai = CreateAIStrategy(role, typeID);
                role.SetAIStrategy(ai);
            }

            // HUD
            var hpBar = CreateHpBarHUD(role.HudSlotCom.HudRoot);
            role.HudSlotCom.SetHpBarHUD(hpBar);

            return role;
        }

        RoleAIStrategy CreateAIStrategy(RoleEntity role, int typeID) {

            // Nodes
            var patrolAIAction = new RolePatrolAIAction();
            patrolAIAction.Inject(role, worldContext);
            BTTreeNode patrolNode = BTTreeFactory.CreateActionNode(patrolAIAction);

            BTTreeNode root = BTTreeFactory.CreateSelectorNode();
            root.AddChild(patrolNode);

            // Tree
            BTTree bt = new BTTree();
            bt.Initialize(root);

            // Strategy
            RoleAIStrategy ai = new RoleAIStrategy();
            ai.Inject(bt);

            return ai;

        }

        public HpBarHUD CreateHpBarHUD(Transform parent) {
            var assetCore = infraContext.AssetCore;
            var hudAssets = assetCore.HUDAssets;
            bool has = hudAssets.TryGet("hud_hp_bar", out GameObject go);
            if (!has) {
                TDLog.Error("Failed to get asset: hud_hp_bar");
                return null;
            }
            var hud = GameObject.Instantiate(go, parent).GetComponent<HpBarHUD>();
            hud.Ctor();
            return hud;
        }

        #endregion

        #region [Item]

        public ItemEntity SpawnItemEntity(int typeID, Vector2 pos) {
            ItemEntity itemEntity = null;

            var templateCore = infraContext.TemplateCore;

            // Template
            if (!templateCore.ItemTemplate.TryGet(typeID, out ItemTM itemTM)) {
                TDLog.Error("Failed to get Item template: " + typeID);
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
            itemEntity.SetID(itemID);
            itemEntity.SetTypeID(typeID);
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
            itemRepo.Add(itemEntity);

            return itemEntity;
        }

        #endregion

    }

}
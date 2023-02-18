using UnityEngine;
using GameArki.BTTreeNS;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Entities;
using TiedanSouls.World.Facades;
using TiedanSouls.Template;

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

            // Skillor
            // Dash / BoomMelee / Infinity
            var skillorTypeIDArray = roleTM.skillorTypeIDArray;
            if (skillorTypeIDArray != null) {
                InitRoleSkillorSlotCom(role, skillorTypeIDArray);
            }

            // HUD
            // HpBar
            var hpBar = CreateHpBarHUD(role.HudSlotCom.HudRoot);
            role.HudSlotCom.SetHpBarHUD(hpBar);

            // Weapon
            // (Weapon Skillor: Melee / HoldMelee / SpecMelee)
            // TODO: weaponTypeID Read From Player's Choice
            int weaponTypeID = 100;
            InitRoleWeaponSlotCom(role, weaponTypeID);

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

        // HUD
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

        // Skillor
        void InitRoleSkillorSlotCom(RoleEntity role, int[] typeIDArray) {
            var idService = worldContext.IDService;
            var templateCore = infraContext.TemplateCore;

            var len = typeIDArray.Length;
            for (int i = 0; i < len; i++) {
                int skillorID = idService.PickSkillorID();
                var skillor = new SkillorModel(skillorID, role);
                var typeID = typeIDArray[i];
                bool has = templateCore.SkillorTemplate.TryGet(typeID, out SkillorTM skillorTM);
                if (!has) {
                    TDLog.Error($"Failed to get skillor template: typeID {typeID}");
                    return;
                }

                skillor.FromTM(skillorTM);
                role.SkillorSlotCom.Add(skillor);
            }
        }

        // Weapon
        void InitRoleWeaponSlotCom(RoleEntity role, int typeID) {

            var weapon = SpawnWeaponModel(typeID);
            if (weapon == null) {
                TDLog.Error("Failed to spawn weapon: " + typeID);
                return;
            }

            var mod = weapon.Mod;
            mod.transform.SetParent(role.WeaponSlotCom.WeaponRoot, false);
            role.WeaponSlotCom.SetWeapon(weapon);
        }

        public WeaponModel SpawnWeaponModel(int typeID) {
            WeaponModel weapon = new WeaponModel();

            var assetCore = infraContext.AssetCore;
            var templateCore = infraContext.TemplateCore;

            // Weapon TM
            bool has = templateCore.WeaponTemplate.TryGet(typeID, out WeaponTM weaponTM);
            if (!has) {
                TDLog.Error("Failed to get weapon template: " + typeID);
                return null;
            }

            // Weapon Mod
            has = assetCore.WeaponModAssets.TryGet(weaponTM.meshName, out GameObject weaponModPrefab);
            if (!has) {
                TDLog.Error("Failed to get weapon mod: " + weaponTM.meshName);
                return null;
            }

            weapon.SetWeaponType(weaponTM.weaponType);
            weapon.SetTypeID(weaponTM.typeID);
            weapon.atk = weaponTM.atk;
            weapon.def = weaponTM.def;
            weapon.crit = weaponTM.crit;
            weapon.skillorMeleeTypeID = weaponTM.skillorMeleeTypeID;
            weapon.skillorHoldMeleeTypeID = weaponTM.skillorHoldMeleeTypeID;
            weapon.skillorSpecMeleeTypeID = weaponTM.skillorSpecMeleeTypeID;

            var go = GameObject.Instantiate(weaponModPrefab);
            weapon.SetMod(go);

            return weapon;
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
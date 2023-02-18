using UnityEngine;
using GameArki.BTTreeNS;
using TiedanSouls.Asset;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Entities;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Service;
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

        public FieldEntity CreateFieldEntity(int typeID) {
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
            entity.SetFieldType(fieldTM.fieldType);

            return entity;
        }

        public RoleEntity CreateRoleEntity(RoleControlType controlType, int typeID, sbyte ally, Vector2 pos) {
            var idService = worldContext.IDService;
            var templateCore = infraContext.TemplateCore;
            var assetCore = infraContext.AssetCore;

            // - Entity
            var containerModAssets = assetCore.ContainerModAssets;
            var contanerAssetName = "mod_container_role";
            bool has = containerModAssets.TryGet(contanerAssetName, out GameObject go);
            if (!has) {
                TDLog.Error($"Failed to get asset: {contanerAssetName}");
                return null;
            }

            var role = GameObject.Instantiate(go).GetComponent<RoleEntity>();
            role.Ctor();

            // - ID
            int id = idService.PickRoleID();
            role.SetID(id);

            // - TM
            has = templateCore.RoleTemplate.TryGet(typeID, out RoleTM roleTM);
            if (!has) {
                TDLog.Error("Failed to get role template: " + typeID);
                return null;
            }

            // - Mod
            var roleModAssets = assetCore.RoleModAssets;
            has = roleModAssets.TryGet(roleTM.modName, out GameObject roleModPrefab);
            if (!has) {
                TDLog.Error("Failed to get sprite: " + roleTM.modName);
                return null;
            }
            GameObject roleMod = GameObject.Instantiate(roleModPrefab, role.Body);
            role.SetMod(roleMod);

            // - Attr
            var attrCom = role.AttrCom;
            attrCom.InitializeHealth(roleTM.hpMax, roleTM.hpMax, roleTM.epMax, roleTM.epMax, roleTM.gpMax, roleTM.gpMax);
            attrCom.InitializeLocomotion(roleTM.moveSpeed, roleTM.jumpSpeed, roleTM.fallingAcceleration, roleTM.fallingSpeedMax);

            // - Pos
            role.SetPos(pos);

            // - Ally
            role.SetAlly(ally);

            // - ControlType
            role.SetControlType(controlType);

            // ==== AI ====
            if (controlType == RoleControlType.AI) {
                var ai = CreateAIStrategy(role, typeID);
                role.SetAIStrategy(ai);
            }

            // ==== Skillor ====
            // Dash / BoomMelee / Infinity
            if (roleTM.skillorTypeIDArray != null) {
                foreach (var skillorTypeID in roleTM.skillorTypeIDArray) {
                    CreateSkillor(role, skillorTypeID);
                }
            }

            // ==== HUD ====
            // HpBar
            var hpBar = CreateHpBarHUD(role.HudSlotCom.HudRoot);
            role.HudSlotCom.SetHpBarHUD(hpBar);

            // ==== Weapon ====
            // (Weapon Skillor: Melee / HoldMelee / SpecMelee)
            CreateWeapon(role, 100);

            return role;
        }

        // ==== AI ====
        RoleAIStrategy CreateAIStrategy(RoleEntity role, int typeID) {

            // - Nodes
            var patrolAIAction = new RolePatrolAIAction();
            patrolAIAction.Inject(role, worldContext);
            BTTreeNode patrolNode = BTTreeFactory.CreateActionNode(patrolAIAction);

            BTTreeNode root = BTTreeFactory.CreateSelectorNode();
            root.AddChild(patrolNode);

            // - Tree
            BTTree bt = new BTTree();
            bt.Initialize(root);

            // - Strategy
            RoleAIStrategy ai = new RoleAIStrategy();
            ai.Inject(bt);

            return ai;

        }

        // ==== Skillor ====
        void CreateSkillor(RoleEntity role, int typeID) {
            var idService = worldContext.IDService;
            var templateCore = infraContext.TemplateCore;
            int skillorID = idService.PickSkillorID();
            var skillor = new SkillorModel(skillorID, role);
            bool has = templateCore.SkillorTemplate.TryGet(typeID, out SkillorTM skillorTM);
            if (!has) {
                TDLog.Error("Failed to get skillor template: " + typeID);
                return;
            }
            skillor.FromTM(skillorTM);
            role.SkillorSlotCom.Add(skillor);
        }

        // ==== Weapon ====
        void CreateWeapon(RoleEntity role, int typeID) {
            var assetCore = infraContext.AssetCore;
            var templateCore = infraContext.TemplateCore;

            // Weapon TM
            bool has = templateCore.WeaponTemplate.TryGet(typeID, out WeaponTM weaponTM);
            if (!has) {
                TDLog.Error("Failed to get weapon template: " + typeID);
                return;
            }

            // Weapon Mod
            has = assetCore.WeaponModAssets.TryGet(weaponTM.meshName, out GameObject weaponModPrefab);
            if (!has) {
                TDLog.Error("Failed to get weapon mod: " + weaponTM.meshName);
                return;
            }

            var weapon = new WeaponModel();
            weapon.weaponType = weaponTM.weaponType;
            weapon.typeID = weaponTM.typeID;
            weapon.atk = weaponTM.atk;
            weapon.def = weaponTM.def;
            weapon.crit = weaponTM.crit;
            weapon.skillorMeleeTypeID = weaponTM.skillorMeleeTypeID;
            weapon.skillorHoldMeleeTypeID = weaponTM.skillorHoldMeleeTypeID;
            weapon.skillorSpecMeleeTypeID = weaponTM.skillorSpecMeleeTypeID;
            var weaponMod = GameObject.Instantiate(weaponModPrefab, role.WeaponSlotCom.WeaponRoot);
            weapon.SetMod(weaponMod);
            role.WeaponSlotCom.SetWeapon(weapon);
        }

        // ==== HUD ====
        // - HpBar
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

    }

}
using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Entities;
using TiedanSouls.Template;

namespace TiedanSouls.World.Domain {

    public class WorldRoleDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldRoleDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public RoleEntity SpawnRole(int typeID, sbyte ally, Vector2 pos) {

            var idService = worldContext.IDService;
            var templateCore = infraContext.TemplateCore;

            // - Entity
            var worldAssets = infraContext.AssetCore.WorldAssets;
            bool has = worldAssets.TryGet("entity_role", out GameObject go);
            if (!has) {
                TDLog.Error("Failed to get asset: entity_role");
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

            // - Mesh
            var spriteAssets = infraContext.AssetCore.SpriteAssets;
            has = spriteAssets.TryGet(roleTM.meshName, out Sprite sprite);
            if (!has) {
                TDLog.Error("Failed to get sprite: " + roleTM.meshName);
                return null;
            }
            role.SetMesh(sprite);

            // - Move
            var attrCom = role.AttrCom;
            attrCom.InitializeHealth(roleTM.hpMax, roleTM.hpMax, roleTM.epMax, roleTM.epMax, roleTM.gpMax, roleTM.gpMax);
            attrCom.InitializeLocomotion(roleTM.moveSpeed, roleTM.jumpSpeed, roleTM.fallingAcceleration, roleTM.fallingSpeedMax);

            // - Pos
            role.SetPos(pos);

            // - Ally
            role.SetAlly(ally);

            // - Skillor
            // Dash / BoomMelee / Infinity
            if (roleTM.skillorTypeIDArray != null) {
                foreach (var skillorTypeID in roleTM.skillorTypeIDArray) {
                    var skillor = new SkillorModel(role);
                    has = templateCore.SkillorTemplate.TryGet(skillorTypeID, out SkillorTM skillorTM);
                    if (!has) {
                        TDLog.Error("Failed to get skillor template: " + skillorTypeID);
                        return null;
                    }
                    skillor.FromTM(skillorTM);
                    skillor.OnTriggerEnterHandle += OnSkillorTriggerEnter;
                    role.SkillorSlotCom.Add(skillor);
                }
            }

            // - Weapon
            // Weapon Mod
            has = infraContext.AssetCore.WeaponAssets.TryGet("mod_spear", out GameObject weaponModPrefab);
            if (!has) {
                TDLog.Error("Failed to get weapon mod: " + "mod_spear");
                return null;
            }

            // Weapon TM
            has = templateCore.WeaponTemplate.TryGet(100, out WeaponTM weaponTM);
            if (!has) {
                TDLog.Error("Failed to get weapon template: " + 100);
                return null;
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

            // Weapon Skillor: Melee / HoldMelee / SpecMelee

            // - Physics
            role.OnFootCollisionEnterHandle += OnRoleFootCollisionEnter;
            role.OnBodyTriggerExitHandle += OnRoleFootTriggerExit;

            // - FSM
            role.FSMCom.EnterIdle();

            var repo = worldContext.RoleRepo;
            repo.Add(role);

            return role;
        }

        // ==== Physics Event ====
        void OnRoleFootCollisionEnter(RoleEntity role, Collision2D other) {
            if (other.gameObject.layer == LayerCollection.GROUND) {
                role.EnterGround();
            } else if (other.gameObject.layer == LayerCollection.CROSS_PLATFORM) {
                role.EnterCrossPlatform();
            }
        }

        void OnRoleFootTriggerExit(RoleEntity role, Collider2D other) {
            if (other.gameObject.layer == LayerCollection.CROSS_PLATFORM) {
                role.LeaveCrossPlatform();
            }
        }

        void OnSkillorTriggerEnter(SkillorModel skillor, Collider2D other) {
            var go = other.gameObject;
            var otherRole = go.GetComponent<RoleEntity>();
            if (otherRole != null) {
                RoleHitRole(skillor, otherRole);
            }
        }

        // ==== Input ====
        public void RecordOwnerInput(RoleEntity ownerRole) {

            var inputGetter = infraContext.InputCore.Getter;
            var inputRecordCom = ownerRole.InputRecordCom;

            // - Move
            Vector2 moveAxis = Vector2.zero;
            if (inputGetter.GetPressing(InputKeyCollection.MOVE_LEFT)) {
                moveAxis.x = -1;
            } else if (inputGetter.GetPressing(InputKeyCollection.MOVE_RIGHT)) {
                moveAxis.x = 1;
            } else {
                moveAxis.x = 0;
            }

            if (inputGetter.GetPressing(InputKeyCollection.MOVE_DOWN)) {
                moveAxis.y = -1;
            } else if (inputGetter.GetPressing(InputKeyCollection.MOVE_UP)) {
                moveAxis.y = 1;
            } else {
                moveAxis.y = 0;
            }
            inputRecordCom.SetMoveAxis(moveAxis);

            // - Jump
            bool isJump = inputGetter.GetDown(InputKeyCollection.JUMP);
            inputRecordCom.SetJumping(isJump);

            // - Melee && HoldMelee
            bool isMelee = inputGetter.GetDown(InputKeyCollection.MELEE);
            inputRecordCom.SetMelee(isMelee);

            // - SpecMelee
            bool isSpecMelee = inputGetter.GetDown(InputKeyCollection.SPEC_MELEE);
            inputRecordCom.SetSpecMelee(isSpecMelee);

            // - BoomMelee
            bool isBoomMelee = inputGetter.GetDown(InputKeyCollection.BOOM_MELEE);
            inputRecordCom.SetBoomMelee(isBoomMelee);

            // - Infinity
            bool isInfinity = inputGetter.GetDown(InputKeyCollection.INFINITY);
            inputRecordCom.SetInfinity(isInfinity);

            // - Dash
            bool isDash = inputGetter.GetDown(InputKeyCollection.DASH);
            inputRecordCom.SetDash(isDash);

        }

        // ==== Locomotion ====
        public void Move(RoleEntity role) {
            role.Move();
        }

        public void Dash(RoleEntity role, Vector2 dir, Vector2 force) {
            role.Dash(dir, force);
        }

        public void Jump(RoleEntity role) {
            role.Jump();
        }

        public void CrossDown(RoleEntity role) {
            role.CrossDown();
        }

        public void Falling(RoleEntity role, float dt) {
            role.Falling(dt);
        }

        // ==== Cast ====
        public void CastByInput(RoleEntity role) {

            // Allowed When Idle
            var fsm = role.FSMCom;
            if (fsm.Status != RoleFSMStatus.Idle) {
                return;
            }

            // Not Allowed When Dead || Hurt
            // Need Cancel When Casting

            var inputRecordCom = role.InputRecordCom;
            if (inputRecordCom.IsSpecMelee) {
                CastByType(role, SkillorType.SpecMelee);
            } else if (inputRecordCom.IsBoomMelee) {
                CastByType(role, SkillorType.BoomMelee);
            } else if (inputRecordCom.IsInfinity) {
                CastByType(role, SkillorType.Infinity);
            } else if (inputRecordCom.IsDash) {
                CastByType(role, SkillorType.Dash);
            } else if (inputRecordCom.IsMelee) {
                CastByType(role, SkillorType.Melee);
            }
        }

        void CastByType(RoleEntity role, SkillorType skillorType) {
            var skillorSlotCom = role.SkillorSlotCom;
            bool has = skillorSlotCom.TryGetByType(skillorType, out var skillor);
            if (!has) {
                TDLog.Error("Failed to get skillor: " + skillorType);
                return;
            }

            // TDLog.Log("Cast: " + skillorType + " -> " + role.ID + " -> " + skillor.TypeID);

            Cast(role, skillor);
        }

        void Cast(RoleEntity role, SkillorModel skillor) {
            var fsm = role.FSMCom;
            fsm.EnterCasting(skillor);
        }

        // ==== Hit ====
        void RoleHitRole(SkillorModel skillor, RoleEntity other) {

            var cur = skillor.Owner;

            // Me Check
            if (cur == other) {
                return;
            }

            // Ally Check
            if (cur.Ally == other.Ally) {
                return;
            }

            // Weapon Damage
            var curWeapon = cur.WeaponSlotCom.Weapon;
            other.HitBeHurt(curWeapon.atk);

            TDLog.Log("OnSkillorTriggerEnter: " + skillor.TypeID + " -> " + other.ID);
            TDLog.Log($"Cur: {cur.ID} Hurt: {other.ID}, other hp Left: {other.AttrCom.HP}");

            if (other.AttrCom.HP <= 0) {
                RoleDie(other);
            }

        }

        void RoleDie(RoleEntity role) {
            var fsm = role.FSMCom;
            fsm.EnterDead();
        }

    }
}